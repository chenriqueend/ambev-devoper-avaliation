using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories
{
    public interface IBranchRepository
    {
        Task<Branch> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Branch>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Branch branch, CancellationToken cancellationToken = default);
        Task UpdateAsync(Branch branch, CancellationToken cancellationToken = default);
        Task DeleteAsync(Branch branch, CancellationToken cancellationToken = default);
    }
}