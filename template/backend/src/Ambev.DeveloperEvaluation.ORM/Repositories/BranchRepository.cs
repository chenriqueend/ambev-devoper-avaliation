using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM.Context;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class BranchRepository : Repository<Branch>, IBranchRepository
{
    public BranchRepository(DeveloperEvaluationContext context) : base(context)
    {
    }

    public override async Task<Branch> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await base.GetByIdAsync(id, cancellationToken);
    }

    public override async Task<IEnumerable<Branch>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await base.GetAllAsync(cancellationToken);
    }

    public override async Task AddAsync(Branch entity, CancellationToken cancellationToken = default)
    {
        await base.AddAsync(entity, cancellationToken);
    }

    public override async Task UpdateAsync(Branch entity, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(entity, cancellationToken);
    }

    public override async Task DeleteAsync(Branch entity, CancellationToken cancellationToken = default)
    {
        await base.DeleteAsync(entity, cancellationToken);
    }
}