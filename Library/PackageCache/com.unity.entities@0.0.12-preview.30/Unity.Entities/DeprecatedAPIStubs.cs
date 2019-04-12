using System;
using System.ComponentModel;

namespace Unity.Entities
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("ComponentDataArray is deprecated. Use IJobForEach or EntityQuery ToComponentDataArray/CopyFromComponentDataArray APIs instead. More information: https://forum.unity.com/threads/api-deprecation-faq-0-0-23.636994/", true)]
    public unsafe struct ComponentDataArray<T> where T : struct, IComponentData { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("SharedComponentDataArray is deprecated. Use ArchetypeChunk.GetSharedComponentData. More information: https://forum.unity.com/threads/api-deprecation-faq-0-0-23.636994/", true)]
    public struct SharedComponentDataArray<T> where T : struct, ISharedComponentData { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("BufferArray is deprecated. Use ArchetypeChunk.GetBufferAccessor() instead. More information: https://forum.unity.com/threads/api-deprecation-faq-0-0-23.636994/", true)]
    public unsafe struct BufferArray<T> where T : struct, IBufferElementData { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("EntityArray is deprecated. Use IJobForEachWithEntity or EntityQuery.ToEntityArray(...) instead. More information: https://forum.unity.com/threads/api-deprecation-faq-0-0-23.636994/", true)]
    public unsafe struct EntityArray { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("ComponentGroupArray has been deprecated. Use Entities.ForEach instead to access managed components. More information: https://forum.unity.com/threads/api-deprecation-faq-0-0-23.636994/", true)]
    public struct ComponentGroupArray<T> where T : struct { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Field)]
    [Obsolete("Injection API is deprecated. For struct injection, use the EntityQuery API instead. For ComponentDataFromEntity injection use (Job)ComponentSystem.GetComponentDataFromEntity. For system injection, use World.GetOrCreateManager(). More information: https://forum.unity.com/threads/api-deprecation-faq-0-0-23.636994/", true)]
    public class InjectAttribute : Attribute { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("CopyComponentData is deprecated. Use EntityQuery.ToComponentDataArray instead. More information: https://forum.unity.com/threads/api-deprecation-faq-0-0-23.636994/", true)]
    public struct CopyComponentData<T>
        where T : struct, IComponentData { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("CopyEntities is deprecated. Use EntityQuery.ToEntityArray instead. More information: https://forum.unity.com/threads/api-deprecation-faq-0-0-23.636994/", true)]
    public struct CopyEntities { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("GameObjectArray has been deprecated. Use ComponentSystem.ForEach or ToTransformArray()[index].gameObject to access entity GameObjects. More information: https://forum.unity.com/threads/api-deprecation-faq-0-0-23.636994/", true)]
    public struct GameObjectArray { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("ComponentArray has been deprecated. Use ComponentSystem.ForEach to access managed components. More information: https://forum.unity.com/threads/api-deprecation-faq-0-0-23.636994/", true)]
    public struct ComponentArray<T> where T: UnityEngine.Component { }

    public static class ComponentGroupExtensionsForGameObjectArray
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("GetGameObjectArray has been deprecated. Use ComponentSystem.ForEach or ToTransformArray()[index].gameObject to access entity GameObjects. More information: https://forum.unity.com/threads/api-deprecation-faq-0-0-23.636994/", true)]
        public static void GetGameObjectArray(this ComponentGroup group) { }
    }
}
