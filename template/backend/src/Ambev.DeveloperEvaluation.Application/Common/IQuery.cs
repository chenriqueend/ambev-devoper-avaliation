using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Common
{
    public interface IQuery<T> : IRequest<QueryResult<T>>
    {
    }
}