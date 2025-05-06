using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Common
{
    public interface IQueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, QueryResult<TResponse>>
        where TRequest : IRequest<QueryResult<TResponse>>
    {
    }
}