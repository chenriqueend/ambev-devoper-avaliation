using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Common
{
    public interface ICommand<T> : IRequest<CommandResult<T>>
    {
    }
}