using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Common
{
    public interface ICommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, CommandResult<TResponse>>
        where TRequest : IRequest<CommandResult<TResponse>>
    {
    }
}