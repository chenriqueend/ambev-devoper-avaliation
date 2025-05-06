using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.ORM.Repositories
{
    public class SaleRepository : Repository<Sale>, ISaleRepository
    {
        private readonly ILogger<SaleRepository> _logger;

        public SaleRepository(DeveloperEvaluationContext context, ILogger<SaleRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public override async Task<Sale> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("[SaleRepository] Getting sale by id {SaleId}", id);
                var sale = await DbSet
                    .Include(s => s.Items)
                    .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

                if (sale == null)
                    throw new KeyNotFoundException($"Sale with ID {id} not found.");

                return sale;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SaleRepository] Error getting sale {SaleId}", id);
                throw;
            }
        }

        public override async Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(s => s.Items)
                .ToListAsync(cancellationToken);
        }

        public override async Task AddAsync(Sale entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("[SaleRepository] Starting to add sale {SaleId}", entity.Id);
                _logger.LogInformation("[SaleRepository] Sale details - Number: {SaleNumber}, Date: {SaleDate}, CustomerId: {CustomerId}, BranchId: {BranchId}, Total: {TotalWithDiscount}",
                    entity.SaleNumber, entity.SaleDate, entity.CustomerId, entity.BranchId, entity.TotalWithDiscount);

                _logger.LogInformation("[SaleRepository] Adding sale to DbSet");
                await DbSet.AddAsync(entity, cancellationToken);
                _logger.LogInformation("[SaleRepository] Sale added to DbSet successfully");

                _logger.LogInformation("[SaleRepository] Configuring {ItemCount} sale items", entity.Items.Count);
                foreach (var item in entity.Items)
                {
                    _logger.LogInformation("[SaleRepository] Configuring sale item - Id: {ItemId}, ProductId: {ProductId}, Quantity: {Quantity}, UnitPrice: {UnitPrice}, TotalAmount: {TotalAmount}, SaleId: {SaleId}, Discount: {Discount}",
                        item.Id, item.ProductId, item.Quantity, item.UnitPrice, item.TotalAmount, item.SaleId, item.Discount);

                    try
                    {
                        Context.Entry(item).State = EntityState.Added;
                        _logger.LogInformation("[SaleRepository] Sale item state set to Added successfully");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[SaleRepository] Error setting item state for item {ItemId}", item.Id);
                        throw;
                    }
                }

                _logger.LogInformation("[SaleRepository] Attempting to save changes");
                try
                {
                    await Context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("[SaleRepository] Changes saved successfully");
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "[SaleRepository] Database update error while saving changes");
                    _logger.LogError("[SaleRepository] Inner exception: {InnerException}", dbEx.InnerException?.Message);
                    throw;
                }

                _logger.LogInformation("[SaleRepository] Successfully added sale {SaleId}", entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SaleRepository] Error adding sale {SaleId}", entity.Id);
                _logger.LogError("[SaleRepository] Error details: {ErrorDetails}", ex.ToString());
                throw;
            }
        }

        public override async Task UpdateAsync(Sale entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("[SaleRepository] Starting to update sale {SaleId}", entity.Id);

                var existingSale = await DbSet
                    .Include(s => s.Items)
                    .FirstOrDefaultAsync(s => s.Id == entity.Id, cancellationToken);

                if (existingSale == null)
                {
                    _logger.LogWarning("[SaleRepository] Sale {SaleId} not found for update", entity.Id);
                    return;
                }

                foreach (var item in entity.Items)
                {
                    var existingItem = existingSale.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
                    if (existingItem != null)
                    {
                        _logger.LogInformation("[SaleRepository] Updating item for product {ProductId} with quantity {Quantity}, discount {Discount}, total {TotalAmount}",
                            item.ProductId, item.Quantity, item.Discount, item.TotalAmount);

                        existingItem.UpdateQuantity(item.Quantity);
                    }
                }

                var activeItems = existingSale.Items.Where(i => !i.IsCancelled).ToList();
                var totalWithDiscount = activeItems.Sum(i => i.TotalAmount);

                var property = typeof(Sale).GetProperty("TotalWithDiscount");
                if (property != null)
                {
                    property.SetValue(existingSale, totalWithDiscount);
                }

                await Context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("[SaleRepository] Successfully updated sale {SaleId}", entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SaleRepository] Error updating sale {SaleId}", entity.Id);
                throw;
            }
        }

        public override async Task DeleteAsync(Sale entity, CancellationToken cancellationToken = default)
        {
            var existingSale = await DbSet
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == entity.Id, cancellationToken);

            if (existingSale == null)
                return;

            foreach (var item in existingSale.Items)
            {
                Context.Entry(item).State = EntityState.Deleted;
            }

            DbSet.Remove(existingSale);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await DbSet.AnyAsync(s => s.Id == id);
        }

        public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("[SaleRepository] Starting to create sale {SaleId}", sale.Id);
                _logger.LogInformation("[SaleRepository] Sale details - Number: {SaleNumber}, Date: {SaleDate}, CustomerId: {CustomerId}, BranchId: {BranchId}, Total: {TotalWithDiscount}",
                    sale.SaleNumber, sale.SaleDate, sale.CustomerId, sale.BranchId, sale.TotalWithDiscount);

                if (sale.SaleDate == default)
                {
                    _logger.LogInformation("[SaleRepository] Sale date is default, creating new sale with current date");
                    sale = new Sale(
                        sale.SaleNumber,
                        DateTime.UtcNow,
                        sale.CustomerId,
                        sale.CustomerName,
                        sale.BranchId,
                        sale.BranchName,
                        sale.TotalWithDiscount
                    );
                }

                _logger.LogInformation("[SaleRepository] Adding sale to DbSet");
                await DbSet.AddAsync(sale, cancellationToken);
                _logger.LogInformation("[SaleRepository] Sale added to DbSet successfully");

                _logger.LogInformation("[SaleRepository] Configuring {ItemCount} sale items", sale.Items.Count);
                foreach (var item in sale.Items)
                {
                    _logger.LogInformation("[SaleRepository] Configuring sale item - Id: {ItemId}, ProductId: {ProductId}, Quantity: {Quantity}, UnitPrice: {UnitPrice}, TotalAmount: {TotalAmount}, SaleId: {SaleId}, Discount: {Discount}",
                        item.Id, item.ProductId, item.Quantity, item.UnitPrice, item.TotalAmount, item.SaleId, item.Discount);

                    try
                    {
                        Context.Entry(item).State = EntityState.Added;
                        _logger.LogInformation("[SaleRepository] Sale item state set to Added successfully");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[SaleRepository] Error setting item state for item {ItemId}", item.Id);
                        throw;
                    }
                }

                _logger.LogInformation("[SaleRepository] Attempting to save changes");
                try
                {
                    await Context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("[SaleRepository] Changes saved successfully");
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "[SaleRepository] Database update error while saving changes");
                    _logger.LogError("[SaleRepository] Inner exception: {InnerException}", dbEx.InnerException?.Message);
                    throw;
                }

                _logger.LogInformation("[SaleRepository] Successfully created sale {SaleId}", sale.Id);
                return sale;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SaleRepository] Error creating sale {SaleId}", sale.Id);
                _logger.LogError("[SaleRepository] Error details: {ErrorDetails}", ex.ToString());
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var sale = await DbSet
                    .Include(s => s.Items)
                    .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

                if (sale == null)
                    return false;

                var domainEvents = await Context.Set<StoredDomainEvent>()
                    .Where(e => e.SaleId == id)
                    .ToListAsync(cancellationToken);

                foreach (var domainEvent in domainEvents)
                {
                    Context.Entry(domainEvent).State = EntityState.Deleted;
                }

                foreach (var item in sale.Items)
                {
                    Context.Entry(item).State = EntityState.Deleted;
                }

                DbSet.Remove(sale);
                await Context.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> CancelSaleAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("[SaleRepository] Starting to cancel sale {SaleId}", id);

                var sale = await DbSet
                    .Include(s => s.Items)
                    .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

                if (sale == null)
                {
                    _logger.LogWarning("[SaleRepository] Sale {SaleId} not found", id);
                    return false;
                }

                _logger.LogInformation("[SaleRepository] Found sale {SaleId} with IsCancelled={IsCancelled}", id, sale.IsCancelled);

                if (sale.IsCancelled)
                {
                    _logger.LogWarning("[SaleRepository] Sale {SaleId} is already cancelled", id);
                    return false;
                }

                sale.Cancel();

                // Add domain events to context
                foreach (var domainEvent in sale.DomainEvents)
                {
                    await Context.Set<StoredDomainEvent>().AddAsync(domainEvent, cancellationToken);
                }

                await Context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("[SaleRepository] Successfully cancelled sale {SaleId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SaleRepository] Error in CancelSaleAsync for sale {SaleId}", id);
                throw;
            }
        }
    }
}