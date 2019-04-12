using System;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;

public class NativeListDeferredArrayTests
{
    struct AliasJob : IJob
    {
        public NativeArray<int> array;
        public NativeList<int> list;

        public void Execute()
        {
        }
    }
    
    struct SetListLengthJob : IJob
    {
        public int ResizeLength;
        public NativeList<int> list;

        public void Execute()
        {
            list.ResizeUninitialized(ResizeLength);
        }
    }
    
    struct SetArrayValuesJobParallel : IJobParallelFor
    {
        public NativeArray<int> array;

        public void Execute(int index)
        {
            array[index] = array.Length;
        }
    }
    
    struct GetArrayValuesJobParallel : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<int> array;

        public void Execute(int index)
        {
        }
    }

    
    struct ParallelForWithoutList : IJobParallelFor
    {
        public void Execute(int index)
        {
        }
    }

    [Test]
    public void ResizedListToDeferredJobArray([Values(0, 1, 2, 3, 4, 5, 6, 42, 97, 1023)]int length)
    {
        var list = new NativeList<int> (Allocator.TempJob);

        var setLengthJob = new SetListLengthJob { list = list, ResizeLength = length };
        var jobHandle = setLengthJob.Schedule();

        var setValuesJob = new SetArrayValuesJobParallel { array = list.AsDeferredJobArray() };
        setValuesJob.Schedule(list, 3, jobHandle).Complete();
        
        Assert.AreEqual(length, list.Length);
        for (int i = 0;i != list.Length;i++)
            Assert.AreEqual(length, list[i]);

        list.Dispose ();
    }
    
    [Test]
    public void ResizeListBeforeSchedule([Values(5)]int length)
    {
        var list = new NativeList<int> (Allocator.TempJob);

        var setLengthJob = new SetListLengthJob { list = list, ResizeLength = length }.Schedule();
        var setValuesJob = new SetArrayValuesJobParallel { array = list.AsDeferredJobArray() };
		setLengthJob.Complete();

        setValuesJob.Schedule(list, 3).Complete();
        
        Assert.AreEqual(length, list.Length);
        for (int i = 0;i != list.Length;i++)
            Assert.AreEqual(length, list[i]);

        list.Dispose ();
    }
    
    [Test]
    public void ResizedListToDeferredJobArray()
    {
        var list = new NativeList<int> (Allocator.TempJob);
        list.Add(1);
        
        var array = list.AsDeferredJobArray();
#pragma warning disable 0219 // assigned but its value is never used
        Assert.Throws<IndexOutOfRangeException>(() => { var value = array[0]; });
#pragma warning restore 0219
        Assert.AreEqual(0, array.Length);

        list.Dispose ();
    }
    
    [Test]
    public void ResizeListWhileJobIsRunning()
    {
        var list = new NativeList<int> (Allocator.TempJob);
        list.ResizeUninitialized(42);

        var setValuesJob = new GetArrayValuesJobParallel { array = list.AsDeferredJobArray() };
        var jobHandle = setValuesJob.Schedule(list, 3);
        
        Assert.Throws<InvalidOperationException>(() => list.ResizeUninitialized(1) );

        jobHandle.Complete();
        list.Dispose ();
    }
    
    
    [Test]
    public void AliasArrayThrows()
    {
        var list = new NativeList<int> (Allocator.TempJob);
        
        var aliasJob = new AliasJob{ list = list, array = list.AsDeferredJobArray() };
        Assert.Throws<InvalidOperationException>(() => aliasJob.Schedule() );

        list.Dispose ();
    }
    
    [Test]
    public void DeferredListCantBeDeletedWhileJobIsRunning()
    {
        var list = new NativeList<int> (Allocator.TempJob);

        var job = new ParallelForWithoutList();
        Assert.Throws<InvalidOperationException>(() => job.Schedule(list, 64) );

        list.Dispose();
    }
}
