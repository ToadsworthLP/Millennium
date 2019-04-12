using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Entities
{
    public sealed unsafe partial class EntityManager
    {
        // The script updater has some bugs that make it unable to handle these as automatic updates
        [Obsolete("CreateComponentGroup has been renamed to CreateEntityQuery.", true)]
        public ComponentGroup CreateComponentGroup(params ComponentType[] requiredComponents)
        {
            throw new NotImplementedException();
        }

        [Obsolete("CreateComponentGroup has been renamed to CreateEntityQuery.", true)]
        public ComponentGroup CreateComponentGroup(params EntityArchetypeQuery[] queriesDesc)
        {
            throw new NotImplementedException();
        }

        [Obsolete("UniversalGroup has been renamed to UniversalQuery. (UnityUpgradable) -> UniversalQuery", true)]
        public EntityQuery UniversalGroup => null;
    }

    [Obsolete("EntityArchetypeQuery has been renamed to EntityQueryDesc. (UnityUpgradable) -> EntityQueryDesc", true)]
    public class EntityArchetypeQuery
    {
        public ComponentType[] Any = null;
        public ComponentType[] None = null;
        public ComponentType[] All = null;
        public EntityArchetypeQueryOptions Options = EntityArchetypeQueryOptions.Default;
    }

    [Flags]
    [Obsolete("EntityArchetypeQueryOptions has been renamed to EntityQueryOptions. (UnityUpgradable) -> EntityQueryOptions", true)]
    public enum EntityArchetypeQueryOptions
    {
        Default = 0,
        IncludePrefab = 1,
        IncludeDisabled = 2,
        FilterWriteGroup = 4,
    }

    [Obsolete("ComponentGroupExtensionsForComponentArray has been renamed to EntityQueryExtensionsForComponentArray. (UnityUpgradable) -> EntityQueryExtensionsForComponentArray", true)]
    public static class ComponentGroupExtensionsForComponentArray
    {
    }

    [Obsolete("ComponentGroupExtensionsForTransformAccessArray has been renamed to EntityQueryExtensionsForTransformAccessArray. (UnityUpgradable) -> EntityQueryExtensionsForTransformAccessArray", true)]
    public static class ComponentGroupExtensionsForTransformAccessArray
    {
    }

    public unsafe abstract partial class ComponentSystemBase
    {
        //[Obsolete("GetComponentGroup has been renamed to GetEntityQuery. (UnityUpgradable) -> GetEntityQuery(*)", true)]
        [Obsolete("GetComponentGroup has been renamed to GetEntityQuery.", true)]
        protected internal ComponentGroup GetComponentGroup(params ComponentType[] componentTypes)
        {
            throw new NotImplementedException();
        }

        //[Obsolete("GetComponentGroup has been renamed to GetEntityQuery. (UnityUpgradable) -> GetEntityQuery(*)", true)]
        [Obsolete("GetComponentGroup has been renamed to GetEntityQuery.", true)]
        protected ComponentGroup GetComponentGroup(NativeArray<ComponentType> componentTypes)
        {
            throw new NotImplementedException();
        }

        //[Obsolete("GetComponentGroup has been renamed to GetEntityQuery. (UnityUpgradable) -> GetEntityQuery(*)", true)]
        [Obsolete("GetComponentGroup has been renamed to GetEntityQuery.", true)]
        protected internal ComponentGroup GetComponentGroup(params EntityArchetypeQuery[] queryDesc)
        {
            throw new NotImplementedException();
        }

        [Obsolete("GetComponentGroup has been renamed to GetEntityQuery.", true)]
        protected internal ComponentGroup GetComponentGroup(params EntityQueryDesc[] queryDesc)
        {
            throw new NotImplementedException();
        }
    }

#if !UNITY_CSHARP_TINY
    public static partial class JobForEachExtensions
    {
        [Obsolete("GetComponentGroupForIJobForEach has been renamed to GetEntityQueryForIJobForEach. (UnityUpgradable) -> GetEntityQueryForIJobForEach(*)", true)]
        public static ComponentGroup GetComponentGroupForIJobForEach(this ComponentSystemBase system,
            Type jobType)
        {
            throw new NotImplementedException();
        }

        [Obsolete("PrepareComponentGroup has been renamed to PrepareEntityQuery. (UnityUpgradable) -> PrepareEntityQuery<T>(*)", true)]
        public static void PrepareComponentGroup<T>(this T jobData, ComponentSystemBase system)
            where T : struct, JobForEachExtensions.IBaseJobForEach
        {
            throw new NotImplementedException();
        }

        [Obsolete("ScheduleGroup has been renamed to Schedule. (UnityUpgradable) -> Schedule<T>(*)", true)]
        public static JobHandle ScheduleGroup<T>(this T jobData, ComponentGroup cg, JobHandle dependsOn = default(JobHandle))
            where T : struct, IBaseJobForEach
        {
            throw new NotImplementedException();
        }

        [Obsolete("ScheduleGroupSingle has been renamed to ScheduleSingle. (UnityUpgradable) -> ScheduleSingle<T>(*)", true)]
        public static JobHandle ScheduleGroupSingle<T>(this T jobData, ComponentGroup cg, JobHandle dependsOn = default(JobHandle))
            where T : struct, IBaseJobForEach
        {
            throw new NotImplementedException();
        }

        [Obsolete("RunGroup has been renamed to Run. (UnityUpgradable) -> Run<T>(*)", true)]
        public static JobHandle Run<T>(this T jobData, ComponentGroup cg, JobHandle dependsOn = default(JobHandle))
            where T : struct, IBaseJobForEach
        {
            throw new NotImplementedException();
        }
    }
#endif

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("ComponentGroup has been renamed to EntityQuery. (UnityUpgradable) -> EntityQuery", true)]
    public class ComponentGroup : IDisposable
    {
        public bool IsEmptyIgnoreFilter
        {
            get { throw new NotImplementedException(); }
        }

        public int CalculateLength()
        {
            throw new NotImplementedException();
        }

        public NativeArray<ArchetypeChunk> CreateArchetypeChunkArray(Allocator allocator, out JobHandle jobhandle)
        {
            throw new NotImplementedException();
        }

        public NativeArray<ArchetypeChunk> CreateArchetypeChunkArray(Allocator allocator)
        {
            throw new NotImplementedException();
        }


        public NativeArray<Entity> ToEntityArray(Allocator allocator, out JobHandle jobhandle)
        {
            throw new NotImplementedException();
        }

        public NativeArray<Entity> ToEntityArray(Allocator allocator)
        {
            throw new NotImplementedException();
        }

        public NativeArray<T> ToComponentDataArray<T>(Allocator allocator, out JobHandle jobhandle)
            where T : struct, IComponentData
        {
            throw new NotImplementedException();
        }

        public NativeArray<T> ToComponentDataArray<T>(Allocator allocator)
            where T : struct, IComponentData
        {
            throw new NotImplementedException();
        }

        public void CopyFromComponentDataArray<T>(NativeArray<T> componentDataArray)
            where T : struct, IComponentData
        {
            throw new NotImplementedException();
        }

        public void CopyFromComponentDataArray<T>(NativeArray<T> componentDataArray, out JobHandle jobhandle)
            where T : struct, IComponentData
        {
            throw new NotImplementedException();
        }

        public Entity GetSingletonEntity()
        {
            throw new NotImplementedException();
        }

        public T GetSingleton<T>()
            where T : struct, IComponentData
        {
            throw new NotImplementedException();
        }

        public void SetSingleton<T>(T value)
            where T : struct, IComponentData
        {
            throw new NotImplementedException();
        }

        public bool CompareComponents(ComponentType[] componentTypes)
        {
            throw new NotImplementedException();
        }

        public bool CompareComponents(NativeArray<ComponentType> componentTypes)
        {
            throw new NotImplementedException();
        }

        public bool CompareQuery(EntityArchetypeQuery[] queryDesc)
        {
            throw new NotImplementedException();
        }

        public void ResetFilter()
        {
            throw new NotImplementedException();
        }

        public void SetFilter<SharedComponent1>(SharedComponent1 sharedComponent1)
            where SharedComponent1 : struct, ISharedComponentData
        {
            throw new NotImplementedException();
        }

        public void SetFilter<SharedComponent1, SharedComponent2>(SharedComponent1 sharedComponent1,
            SharedComponent2 sharedComponent2)
            where SharedComponent1 : struct, ISharedComponentData
            where SharedComponent2 : struct, ISharedComponentData
        {
            throw new NotImplementedException();
        }

        public void SetFilterChanged(ComponentType componentType)
        {
            throw new NotImplementedException();
        }

        public void SetFilterChanged(ComponentType[] componentType)
        {
            throw new NotImplementedException();
        }

        public void CompleteDependency()
        {
            throw new NotImplementedException();
        }

        public JobHandle GetDependency()
        {
            throw new NotImplementedException();
        }

        public void AddDependency(JobHandle job)
        {
            throw new NotImplementedException();
        }

        public int GetCombinedComponentOrderVersion()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
