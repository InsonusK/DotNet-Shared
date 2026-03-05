using MediatR;

namespace InsonusK.Shared.Command.EntityLoading.Interfaces;

public interface ICommandContextSource
{
    ICommandContext GetFor(IRequest command);
}