using System.Collections.Concurrent;
using System.Windows.Input;
using InsonusK.Shared.Command.EntityLoading.Interfaces;
using InsonusK.Shared.Command.EntityLoading.Tools;
using InsonusK.Shared.Command.Interfaces;
using MediatR;

namespace InsonusK.Shared.Command.EntityLoading.Services;

internal class CommandContextManager : ICommandContextSource
{
    private readonly ConcurrentDictionary<IRequest, CommandContext> _commandContext = new();


    public CommandContext StartFor(IRequest command)
    {
        var context = new CommandContext();
        if (!_commandContext.TryAdd(command, context))
            throw new InvalidOperationException("A context for this command already exists");
        return context;
    }
    public void EndFor(IRequest command)
    {
        if (!_commandContext.TryRemove(command, out _))
            throw new InvalidOperationException("No context found for this command");
    }

    public ICommandContext GetFor(IRequest command)
    {
        return _commandContext[command];
    }
}