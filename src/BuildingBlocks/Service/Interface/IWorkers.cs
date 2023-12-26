using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BuildingBlocks.Service.Interface;
public interface IWorkers
{
    ActionBlock<TInput> AddWork<TInput>(Action<TInput> work) where TInput : class, new();

    ActionBlock<TInput> AddWork<TInput>(Action<TInput> work, ExecutionDataflowBlockOptions dataflowBlockOptions)
        where TInput : class, new();

    ActionBlock<TInput> AddWork<TInput>(Func<TInput, Task> work) where TInput : class, new();

    ActionBlock<TInput> AddWork<TInput>(Func<TInput, Task> work, ExecutionDataflowBlockOptions dataflowBlockOptions)
        where TInput : class, new();
}
