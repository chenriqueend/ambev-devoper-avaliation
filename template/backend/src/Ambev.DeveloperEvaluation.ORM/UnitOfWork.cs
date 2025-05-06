using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.ORM.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ambev.DeveloperEvaluation.ORM;

public class UnitOfWork : IUnitOfWork
{
    private readonly DeveloperEvaluationContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(DeveloperEvaluationContext context)
    {
        _context = context;
    }

    public async Task<bool> Commit()
    {
        const int maxRetries = 3;
        var retryCount = 0;

        while (retryCount < maxRetries)
        {
            try
            {
                var result = await _context.SaveChangesAsync() > 0;
                return result;
            }
            catch (DbUpdateConcurrencyException)
            {
                retryCount++;
                if (retryCount == maxRetries)
                    throw;

                await Task.Delay(100 * retryCount); // Exponential backoff
            }
        }

        return false;
    }

    public async Task Rollback()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}