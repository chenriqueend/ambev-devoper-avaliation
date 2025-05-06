using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Commands.Sales;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Handlers.Sales
{
    public class CreateSaleCommandHandler : ICommandHandler<CreateSaleCommand, Guid>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateSaleCommandHandler> _logger;

        public CreateSaleCommandHandler(
            ISaleRepository saleRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateSaleCommandHandler> logger)
        {
            _saleRepository = saleRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CommandResult<Guid>> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting to create sale with number: {SaleNumber}", request.SaleNumber);
                _logger.LogInformation("Request details - CustomerId: {CustomerId}, BranchId: {BranchId}, Items count: {ItemsCount}",
                    request.CustomerId, request.BranchId, request.Items.Count);

                // Calculate initial total based on items
                var totalQuantity = request.Items.Sum(item => item.Quantity);
                _logger.LogInformation("Total quantity of items: {TotalQuantity}", totalQuantity);

                if (totalQuantity > 20)
                {
                    _logger.LogWarning("Total quantity exceeds 20 items");
                    throw new DomainException("Total quantity across all items cannot exceed 20.");
                }

                // Calculate initial total with discount
                decimal subtotal = request.Items.Sum(item => item.Quantity * item.UnitPrice);
                decimal discountPercentage = totalQuantity >= 10 ? 0.20m : totalQuantity >= 4 ? 0.10m : 0;
                decimal initialTotal = subtotal * (1 - discountPercentage);

                _logger.LogInformation("Calculated values - Subtotal: {Subtotal}, Discount: {DiscountPercentage}%, Initial Total: {InitialTotal}",
                    subtotal, discountPercentage * 100, initialTotal);

                _logger.LogInformation("Creating sale with initial total: {InitialTotal}", initialTotal);

                var sale = new Sale(
                    request.SaleNumber,
                    DateTime.UtcNow,
                    request.CustomerId,
                    request.CustomerName,
                    request.BranchId,
                    request.BranchName,
                    initialTotal
                );

                _logger.LogInformation("Sale entity created with ID: {SaleId}", sale.Id);
                _logger.LogInformation("Sale details - Number: {SaleNumber}, Date: {SaleDate}, CustomerId: {CustomerId}, BranchId: {BranchId}, Total: {TotalWithDiscount}",
                    sale.SaleNumber, sale.SaleDate, sale.CustomerId, sale.BranchId, sale.TotalWithDiscount);

                // Create and add items
                foreach (var item in request.Items)
                {
                    _logger.LogInformation("Creating sale item for product: {ProductId}", item.ProductId);
                    _logger.LogInformation("Item details - Description: {Description}, Quantity: {Quantity}, UnitPrice: {UnitPrice}",
                        item.ProductDescription, item.Quantity, item.UnitPrice);

                    var saleItem = new SaleItem(
                        item.ProductId,
                        item.ProductDescription,
                        item.Quantity,
                        item.UnitPrice,
                        sale
                    );

                    _logger.LogInformation("Sale item created with ID: {ItemId}", saleItem.Id);
                    _logger.LogInformation("Sale item details - Quantity: {Quantity}, UnitPrice: {UnitPrice}, TotalAmount: {TotalAmount}, Discount: {Discount}",
                        saleItem.Quantity, saleItem.UnitPrice, saleItem.TotalAmount, saleItem.Discount);

                    sale.AddItem(saleItem);
                }

                _logger.LogInformation("All items added to sale. Final total with discount: {TotalWithDiscount}", sale.TotalWithDiscount);
                _logger.LogInformation("Sale has {ItemCount} items", sale.Items.Count);

                _logger.LogInformation("Adding sale to repository");
                try
                {
                    await _saleRepository.CreateAsync(sale, cancellationToken);
                    _logger.LogInformation("Sale added to repository successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding sale to repository");
                    throw;
                }

                _logger.LogInformation("Committing changes");
                try
                {
                    await _unitOfWork.Commit();
                    _logger.LogInformation("Changes committed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error committing changes");
                    throw;
                }

                _logger.LogInformation("Sale created successfully with ID: {SaleId}", sale.Id);
                return CommandResult<Guid>.CreateSuccess(sale.Id);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex, "Domain validation error while creating sale");
                return CommandResult<Guid>.CreateFailure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating sale");
                _logger.LogError("Error details: {ErrorDetails}", ex.ToString());
                return CommandResult<Guid>.CreateFailure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}