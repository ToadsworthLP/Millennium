using System;
using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Profiling;

namespace Unity.Entities
{

    /// <summary>
    /// The ComponentJobSafetyManager maintains a safety handle for each component type registered in the TypeManager.
    /// It also maintains JobHandles for each type with any jobs that read or write those component types.
    /// Safety and job handles are only maintained for components that can be modified by jobs:
    /// That means only dynamic buffer components and component data that are not tag components will have valid
    /// safety and job handles. For those components the safety handle represents ReadOnly or ReadWrite access to those
    /// components as well as their change versions.
    /// The Entity type is a special case: It can not be modified by jobs and its safety handle is used to represent the
    /// entire EntityManager state. Any job reading from any part of the EntityManager must contain either a safety handle
    /// for the Entity type OR a safety handle for any other component type.
    /// Job component systems that have no other type dependencies have their JobHandles registered on the Entity type
    /// to ensure that they are completed by CompleteAllJobsAndInvalidateArrays
    /// </summary>
    internal unsafe struct ComponentJobSafetyManager
    {
        private const int kMaxReadJobHandles = 17;
        private const int kMaxTypes = TypeManager.MaximumTypesCount;

        private JobHandle* m_JobDependencyCombineBuffer;
        private int m_JobDependencyCombineBufferCount;
        private ComponentSafetyHandle* m_ComponentSafetyHandles;

        private JobHandle m_ExclusiveTransactionDependency;

        private bool m_HasCleanHandles;

        private JobHandle* m_ReadJobFences;

        private const int EntityTypeIndex = 1;


        public void OnCreate()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_TempSafety = AtomicSafetyHandle.Create();
#endif
            m_ReadJobFences = (JobHandle*) UnsafeUtility.Malloc(sizeof(JobHandle) * kMaxReadJobHandles * kMaxTypes, 16,
                Allocator.Persistent);
            UnsafeUtility.MemClear(m_ReadJobFences, sizeof(JobHandle) * kMaxReadJobHandles * kMaxTypes);

            m_ComponentSafetyHandles =
                (ComponentSafetyHandle*) UnsafeUtility.Malloc(sizeof(ComponentSafetyHandle) * kMaxTypes, 16,
                    Allocator.Persistent);
            UnsafeUtility.MemClear(m_ComponentSafetyHandles, sizeof(ComponentSafetyHandle) * kMaxTypes);

            m_JobDependencyCombineBufferCount = 4 * 1024;
            m_JobDependencyCombineBuffer = (JobHandle*) UnsafeUtility.Malloc(
                sizeof(ComponentSafetyHandle) * m_JobDependencyCombineBufferCount, 16, Allocator.Persistent);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            CreateComponentSafetyHandles(kMaxTypes);
#endif

            m_HasCleanHandles = true;
            IsInTransaction = false;
            m_ExclusiveTransactionDependency = default(JobHandle);
        }

        public bool IsInTransaction { get; private set; }

        public JobHandle ExclusiveTransactionDependency
        {
            get { return m_ExclusiveTransactionDependency; }
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (!IsInTransaction)
                    throw new InvalidOperationException(
                        "EntityManager.TransactionDependency can only after EntityManager.BeginExclusiveEntityTransaction has been called.");

                if (!JobHandle.CheckFenceIsDependencyOrDidSyncFence(m_ExclusiveTransactionDependency, value))
                    throw new InvalidOperationException(
                        "EntityManager.TransactionDependency must depend on the Entity Transaction job.");
#endif
                m_ExclusiveTransactionDependency = value;
            }
        }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        public AtomicSafetyHandle ExclusiveTransactionSafety { get; private set; }
        
        void CreateComponentSafetyHandles(int count)
        {
            for (var i = 0; i != count; i++)
            {
                m_ComponentSafetyHandles[i].SafetyHandle = AtomicSafetyHandle.Create();
                AtomicSafetyHandle.SetAllowSecondaryVersionWriting(m_ComponentSafetyHandles[i].SafetyHandle, false);

                m_ComponentSafetyHandles[i].BufferHandle = AtomicSafetyHandle.Create();
            }
        }

#endif

        //@TODO: Optimize as one function call to in batch bump version on every single handle...
        public void CompleteAllJobsAndInvalidateArrays()
        {
            if (m_HasCleanHandles)
                return;

            Profiler.BeginSample("CompleteAllJobsAndInvalidateArrays");

            var count = TypeManager.GetTypeCount();
            for (var t = 0; t != count; t++)
            {
                m_ComponentSafetyHandles[t].WriteFence.Complete();

                var readFencesCount = m_ComponentSafetyHandles[t].NumReadFences;
                var readFences = m_ReadJobFences + t * kMaxReadJobHandles;
                for (var r = 0; r != readFencesCount; r++)
                    readFences[r].Complete();
                m_ComponentSafetyHandles[t].NumReadFences = 0;
            }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            for (var i = 0; i != count; i++)
            {
                AtomicSafetyHandle.CheckDeallocateAndThrow(m_ComponentSafetyHandles[i].SafetyHandle);
                AtomicSafetyHandle.CheckDeallocateAndThrow(m_ComponentSafetyHandles[i].BufferHandle);
            }

            for (var i = 0; i != count; i++)
            {
                AtomicSafetyHandle.Release(m_ComponentSafetyHandles[i].SafetyHandle);
                AtomicSafetyHandle.Release(m_ComponentSafetyHandles[i].BufferHandle);
            }

            CreateComponentSafetyHandles(count);
#endif

            m_HasCleanHandles = true;

            Profiler.EndSample();
        }



        public void Dispose()
        {
            for (var i = 0; i < kMaxTypes; i++)
                m_ComponentSafetyHandles[i].WriteFence.Complete();

            for (var i = 0; i < kMaxTypes * kMaxReadJobHandles; i++)
                m_ReadJobFences[i].Complete();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            for (var i = 0; i < kMaxTypes; i++)
            {
                var res0 = AtomicSafetyHandle.EnforceAllBufferJobsHaveCompletedAndRelease(m_ComponentSafetyHandles[i].SafetyHandle);
                var res1 = AtomicSafetyHandle.EnforceAllBufferJobsHaveCompletedAndRelease(m_ComponentSafetyHandles[i].BufferHandle);

                if (res0 == EnforceJobResult.DidSyncRunningJobs || res1 == EnforceJobResult.DidSyncRunningJobs)
                    Debug.LogError(
                        "Disposing EntityManager but a job is still running against the ComponentData. It appears the job has not been registered with JobComponentSystem.AddDependency.");
            }

            AtomicSafetyHandle.Release(m_TempSafety);
#endif

            UnsafeUtility.Free(m_JobDependencyCombineBuffer, Allocator.Persistent);

            UnsafeUtility.Free(m_ComponentSafetyHandles, Allocator.Persistent);
            m_ComponentSafetyHandles = null;

            UnsafeUtility.Free(m_ReadJobFences, Allocator.Persistent);
            m_ReadJobFences = null;
        }

        public void CompleteDependenciesNoChecks(int* readerTypes, int readerTypesCount, int* writerTypes, int writerTypesCount)
        {
            for (var i = 0; i != writerTypesCount; i++)
                CompleteReadAndWriteDependencyNoChecks(writerTypes[i]);

            for (var i = 0; i != readerTypesCount; i++)
                CompleteWriteDependencyNoChecks(readerTypes[i]);
        }

        internal void PreDisposeCheck()
        {
            for (var i = 0; i < kMaxTypes; i++)
                m_ComponentSafetyHandles[i].WriteFence.Complete();

            for (var i = 0; i < kMaxTypes * kMaxReadJobHandles; i++)
                m_ReadJobFences[i].Complete();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            for (var i = 0; i < kMaxTypes; i++)
            {
                var res0 = AtomicSafetyHandle.EnforceAllBufferJobsHaveCompleted(m_ComponentSafetyHandles[i].SafetyHandle);
                var res1 = AtomicSafetyHandle.EnforceAllBufferJobsHaveCompleted(m_ComponentSafetyHandles[i].BufferHandle);
                if (res0 == EnforceJobResult.DidSyncRunningJobs || res1 == EnforceJobResult.DidSyncRunningJobs)
                    Debug.LogError(
                        "Disposing EntityManager but a job is still running against the ComponentData. It appears the job has not been registered with JobComponentSystem.AddDependency.");
            }
#endif
        }

        public bool HasReaderOrWriterDependency(int type, JobHandle dependency)
        {
            var typeWithoutFlags = type & TypeManager.ClearFlagsMask;
            var writer = m_ComponentSafetyHandles[typeWithoutFlags].WriteFence;
            if (JobHandle.CheckFenceIsDependencyOrDidSyncFence(dependency, writer))
                return true;

            var count = m_ComponentSafetyHandles[typeWithoutFlags].NumReadFences;
            for (var r = 0; r < count; r++)
            {
                var reader = m_ReadJobFences[typeWithoutFlags * kMaxReadJobHandles + r];
                if (JobHandle.CheckFenceIsDependencyOrDidSyncFence(dependency, reader))
                    return true;
            }

            return false;
        }

        public JobHandle GetDependency(int* readerTypes, int readerTypesCount, int* writerTypes, int writerTypesCount)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (readerTypesCount * kMaxReadJobHandles + writerTypesCount > m_JobDependencyCombineBufferCount)
                throw new ArgumentException("Too many readers & writers in GetDependency");
#endif

            var count = 0;
            for (var i = 0; i != readerTypesCount; i++)
                m_JobDependencyCombineBuffer[count++] = m_ComponentSafetyHandles[readerTypes[i] & TypeManager.ClearFlagsMask].WriteFence;

            for (var i = 0; i != writerTypesCount; i++)
            {
                var writerType = writerTypes[i] & TypeManager.ClearFlagsMask;

                m_JobDependencyCombineBuffer[count++] = m_ComponentSafetyHandles[writerType].WriteFence;

                var numReadFences = m_ComponentSafetyHandles[writerType].NumReadFences;
                for (var j = 0; j != numReadFences; j++)
                    m_JobDependencyCombineBuffer[count++] = m_ReadJobFences[writerType * kMaxReadJobHandles + j];
            }

            return JobHandleUnsafeUtility.CombineDependencies(m_JobDependencyCombineBuffer,
                count);
        }

        public JobHandle AddDependency(int* readerTypes, int readerTypesCount, int* writerTypes, int writerTypesCount,
            JobHandle dependency)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            JobHandle* combinedDependencies = null;
            var combinedDependenciesCount = 0;
#endif
            m_HasCleanHandles = false;

            if (readerTypesCount == 0 && writerTypesCount == 0)
            {
                // if no dependency types are provided add read dependency to the Entity type
                // to ensure these jobs are still synced by CompleteAllJobsAndInvalidateArrays
                m_ReadJobFences[EntityTypeIndex * kMaxReadJobHandles +
                    m_ComponentSafetyHandles[EntityTypeIndex].NumReadFences] = dependency;
                m_ComponentSafetyHandles[EntityTypeIndex].NumReadFences++;

                if (m_ComponentSafetyHandles[EntityTypeIndex].NumReadFences == kMaxReadJobHandles)
                {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    return CombineReadDependencies(EntityTypeIndex);
#else
                    CombineReadDependencies(EntityTypeIndex);
#endif
                }
                return dependency;
            }

            for (var i = 0; i != writerTypesCount; i++)
            {
                var writer = writerTypes[i] & TypeManager.ClearFlagsMask;
                m_ComponentSafetyHandles[writer].WriteFence = dependency;
            }


            for (var i = 0; i != readerTypesCount; i++)
            {
                var reader = readerTypes[i] & TypeManager.ClearFlagsMask;
                m_ReadJobFences[reader * kMaxReadJobHandles + m_ComponentSafetyHandles[reader].NumReadFences] =
                    dependency;
                m_ComponentSafetyHandles[reader].NumReadFences++;

                if (m_ComponentSafetyHandles[reader].NumReadFences == kMaxReadJobHandles)
                {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    var combined = CombineReadDependencies(reader);
                    if (combinedDependencies == null)
                    {
                        JobHandle* temp = stackalloc JobHandle[readerTypesCount];
                        combinedDependencies = temp;
                    }

                    combinedDependencies[combinedDependenciesCount++] = combined;
#else
                    CombineReadDependencies(reader);
#endif
                }
            }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (combinedDependencies != null)
                return JobHandleUnsafeUtility.CombineDependencies(combinedDependencies, combinedDependenciesCount);
            return dependency;
#else
            return dependency;
#endif
        }

        public void CompleteWriteDependencyNoChecks(int type)
        {
            m_ComponentSafetyHandles[type & TypeManager.ClearFlagsMask].WriteFence.Complete();
        }

        public void CompleteReadAndWriteDependencyNoChecks(int type)
        {
            var typeWithoutFlags = type & TypeManager.ClearFlagsMask;

            for (var i = 0; i < m_ComponentSafetyHandles[typeWithoutFlags].NumReadFences; ++i)
                m_ReadJobFences[typeWithoutFlags * kMaxReadJobHandles + i].Complete();
            m_ComponentSafetyHandles[typeWithoutFlags].NumReadFences = 0;

            m_ComponentSafetyHandles[typeWithoutFlags].WriteFence.Complete();
        }

        public void CompleteWriteDependency(int type)
        {
            //TODO: avoid call and turn into assert
            if (TypeManager.IsZeroSized(type))
                return;

            CompleteWriteDependencyNoChecks(type);
            
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var typeWithoutFlags = type & TypeManager.ClearFlagsMask;
            AtomicSafetyHandle.CheckReadAndThrow(m_ComponentSafetyHandles[typeWithoutFlags].SafetyHandle);
            AtomicSafetyHandle.CheckReadAndThrow(m_ComponentSafetyHandles[typeWithoutFlags].BufferHandle);
#endif
            
        }

        public void CompleteReadAndWriteDependency(int type)
        {
            //TODO: avoid call and turn into assert
            if (TypeManager.IsZeroSized(type))
                return;

            CompleteReadAndWriteDependencyNoChecks(type);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var typeWithoutFlags = type & TypeManager.ClearFlagsMask;
            AtomicSafetyHandle.CheckWriteAndThrow(m_ComponentSafetyHandles[typeWithoutFlags].SafetyHandle);
            AtomicSafetyHandle.CheckWriteAndThrow(m_ComponentSafetyHandles[typeWithoutFlags].BufferHandle);
#endif
        }
        
        
#if ENABLE_UNITY_COLLECTIONS_CHECKS

        public AtomicSafetyHandle GetEntityManagerSafetyHandle()
        {
            m_HasCleanHandles = false;
            var handle = m_ComponentSafetyHandles[EntityTypeIndex].SafetyHandle;
            AtomicSafetyHandle.UseSecondaryVersion(ref handle);
            return handle;
        }
        
        public AtomicSafetyHandle GetSafetyHandle(int type, bool isReadOnly)
        {
            if (TypeManager.IsZeroSized(type))
            {
                var entityTypeHandle = m_ComponentSafetyHandles[EntityTypeIndex].SafetyHandle;
                AtomicSafetyHandle.UseSecondaryVersion(ref entityTypeHandle);
                return entityTypeHandle;
            }

            m_HasCleanHandles = false;
            var handle = m_ComponentSafetyHandles[type & TypeManager.ClearFlagsMask].SafetyHandle;
            if (isReadOnly)
                AtomicSafetyHandle.UseSecondaryVersion(ref handle);

            return handle;
        }

        public AtomicSafetyHandle GetBufferSafetyHandle(int type)
        {
            Assert.IsTrue(TypeManager.IsBuffer(type));
            m_HasCleanHandles = false;

            return m_ComponentSafetyHandles[type & TypeManager.ClearFlagsMask].BufferHandle;
        }
#endif

        private JobHandle CombineReadDependencies(int typeWithoutFlags)
        {
            var combined = JobHandleUnsafeUtility.CombineDependencies(
                m_ReadJobFences + typeWithoutFlags * kMaxReadJobHandles, m_ComponentSafetyHandles[typeWithoutFlags].NumReadFences);

            m_ReadJobFences[typeWithoutFlags * kMaxReadJobHandles] = combined;
            m_ComponentSafetyHandles[typeWithoutFlags].NumReadFences = 1;

            return combined;
        }

        public void BeginExclusiveTransaction()
        {
            if (IsInTransaction)
                return;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            for (var i = 0; i != TypeManager.GetTypeCount(); i++)
            {
                AtomicSafetyHandle.CheckDeallocateAndThrow(m_ComponentSafetyHandles[i].SafetyHandle);
                AtomicSafetyHandle.CheckDeallocateAndThrow(m_ComponentSafetyHandles[i].BufferHandle);
            }

            for (var i = 0; i != TypeManager.GetTypeCount(); i++)
            {
                AtomicSafetyHandle.Release(m_ComponentSafetyHandles[i].SafetyHandle);
                AtomicSafetyHandle.Release(m_ComponentSafetyHandles[i].BufferHandle);
            }
#endif

            IsInTransaction = true;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            ExclusiveTransactionSafety = AtomicSafetyHandle.Create();
#endif
            m_ExclusiveTransactionDependency = GetAllDependencies();
        }

        public void EndExclusiveTransaction()
        {
            if (!IsInTransaction)
                return;

            m_ExclusiveTransactionDependency.Complete();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var res = AtomicSafetyHandle.EnforceAllBufferJobsHaveCompletedAndRelease(ExclusiveTransactionSafety);
            if (res != EnforceJobResult.AllJobsAlreadySynced)
                //@TODO: Better message
                Debug.LogError("ExclusiveEntityTransaction job has not been registered");
#endif
            IsInTransaction = false;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            CreateComponentSafetyHandles(TypeManager.GetTypeCount());
#endif
        }

        private JobHandle GetAllDependencies()
        {
            var jobHandles =
                new NativeArray<JobHandle>(TypeManager.GetTypeCount() * (kMaxReadJobHandles + 1), Allocator.Temp);

            var count = 0;
            for (var i = 0; i != TypeManager.GetTypeCount(); i++)
            {
                jobHandles[count++] = m_ComponentSafetyHandles[i].WriteFence;

                var numReadFences = m_ComponentSafetyHandles[i].NumReadFences;
                for (var j = 0; j != numReadFences; j++)
                    jobHandles[count++] = m_ReadJobFences[i * kMaxReadJobHandles + j];
            }

            var combined = JobHandle.CombineDependencies(jobHandles);
            jobHandles.Dispose();

            return combined;
        }

        private struct ComponentSafetyHandle
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            public AtomicSafetyHandle SafetyHandle;
            public AtomicSafetyHandle BufferHandle;
#endif
            public JobHandle WriteFence;
            public int NumReadFences;
        }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_TempSafety;
#endif
    }
}
