using System;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.Infrastructure.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private readonly ApplicationDbContext _context;

        public BranchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Branch> GetByIdAsync(Guid id)
        {
            return await _context.Branches.FindAsync(id);
        }

        public async Task AddAsync(Branch branch)
        {
            await _context.Branches.AddAsync(branch);
        }

        public async Task UpdateAsync(Branch branch)
        {
            var existingBranch = await _context.Branches.FindAsync(branch.Id);
            if (existingBranch == null)
                throw new InvalidOperationException($"Branch with ID {branch.Id} not found.");

            _context.Entry(existingBranch).CurrentValues.SetValues(branch);
        }

        public async Task DeleteAsync(Branch branch)
        {
            var existingBranch = await _context.Branches.FindAsync(branch.Id);
            if (existingBranch == null)
                throw new InvalidOperationException($"Branch with ID {branch.Id} not found.");

            _context.Branches.Remove(existingBranch);
        }
    }
} 