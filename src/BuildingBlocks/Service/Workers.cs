using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using BuildingBlocks.Service.Interface;

namespace BuildingBlocks.Service;
public sealed class Workers : IWorkers
{
    private ConcurrentBag<IDataflowBlock> Actions { get; } = new();

    public ActionBlock<TInput> AddWork<TInput>(Func<TInput, Task> work) where TInput : class, new()
    {
        var action = new ActionBlock<TInput>(work);
        Actions.Add(action);

        return action;
    }

    public ActionBlock<TInput> AddWork<TInput>(Action<TInput> work) where TInput : class, new()
    {
        var action = new ActionBlock<TInput>(work);
        Actions.Add(action);

        return action;
    }

    public ActionBlock<TInput> AddWork<TInput>(Func<TInput, Task> work,
        ExecutionDataflowBlockOptions dataflowBlockOptions) where TInput : class, new()
    {
        var action = new ActionBlock<TInput>(work, dataflowBlockOptions);
        Actions.Add(action);

        return action;
    }

    public ActionBlock<TInput> AddWork<TInput>(Action<TInput> work, ExecutionDataflowBlockOptions dataflowBlockOptions)
        where TInput : class, new()
    {
        var action = new ActionBlock<TInput>(work, dataflowBlockOptions);
        Actions.Add(action);

        return action;
    }
}
