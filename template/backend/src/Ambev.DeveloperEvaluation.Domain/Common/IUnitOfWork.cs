using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Common
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
        Task Rollback();
    }
}