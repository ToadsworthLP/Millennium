using System;
using System.Reflection;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Jobs
{
#if !UNITY_ZEROPLAYER
    public static class IJobParallelForDeferExtensions
    {
        internal struct JobStructNativeList<T> where T: struct, IJobParallelFor
        {
            public T JobData;

            public static IntPtr jobReflectionData;

            public static IntPtr Initialize()
            {
                if (jobReflectionData == IntPtr.Zero)
                    jobReflectionData = JobsUtility.CreateJobReflectionData(typeof(JobStructNativeList<T>), typeof(T), JobType.ParallelFor, (ExecuteJobFunction)Execute);

                return jobReflectionData;
            }

            public delegate void ExecuteJobFunction(ref JobStructNativeList<T> data, System.IntPtr additionalPtr, System.IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);

            public unsafe static void Execute(ref JobStructNativeList<T> self, System.IntPtr additionalPtr, System.IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex)
            {
                while (true)
                {
                    int begin;
                    int end;
                    if (!JobsUtility.GetWorkStealingRange(ref ranges, jobIndex, out begin, out end))
                        break;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    JobsUtility.PatchBufferMinMaxRanges(bufferRangePatchData, UnsafeUtility.AddressOf(ref self.JobData), begin, end - begin);
#endif

                    for (var i = begin; i < end; ++i)
                        self.JobData.Execute(i);
                }

            }

        }

        unsafe public static JobHandle Schedule<T, U>(this T jobData, NativeList<U> list, int innerloopBatchCount, JobHandle dependsOn = new JobHandle()) 
            where T : struct, IJobParallelFor 
            where U : struct
        {
            var fullData = new JobStructNativeList<T>
            {
                JobData = jobData
            };

            var scheduleParams = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref fullData), JobStructNativeList<T>.Initialize(), dependsOn, ScheduleMode.Batched);

            void* atomicSafetyHandlePtr = null;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var safety = NativeListUnsafeUtility.GetAtomicSafetyHandle(ref list);
            atomicSafetyHandlePtr = UnsafeUtility.AddressOf(ref safety);
#endif
            return JobsUtility.ScheduleParallelForDeferArraySize(ref scheduleParams, innerloopBatchCount, NativeListUnsafeUtility.GetInternalListDataPtrUnchecked(ref list), atomicSafetyHandlePtr);
        }
/*
        unsafe public static void Run<T, U>(this T jobData, NativeList<U> list, int innerloopBatchCount) where T : struct, IJobParallelFor
        {
            var scheduleParams = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref jobData), ParallelForJobStruct<T>.Initialize(), new JobHandle(), ScheduleMode.Run);
            return JobsUtility.ScheduleParallelFor(ref scheduleParams, innerloopBatchCount, list.m_Buffer);
        }
*/
    }
#endif
}
