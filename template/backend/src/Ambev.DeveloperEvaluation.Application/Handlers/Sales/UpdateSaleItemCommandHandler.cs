using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Commands.Sales;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Handlers.Sales
{
    public class UpdateSaleItemCommandHandler : IRequestHandler<UpdateSaleItemCommand, CommandResult<bool>>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<UpdateSaleItemCommandHandler> _logger;

        public UpdateSaleItemCommandHandler(
            ISaleRepository saleRepository,
            ILogger<UpdateSaleItemCommandHandler> logger)
        {
            _saleRepository = saleRepository;
            _logger = logger;
        }

        public async Task<CommandResult<bool>> Handle(UpdateSaleItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("[UpdateSaleItemCommandHandler] Starting to update item quantity for sale {SaleId} and product {ProductId}",
                    request.SaleId, request.ProductId);

                var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
                if (sale == null)
                {
                    _logger.LogWarning("[UpdateSaleItemCommandHandler] Sale {SaleId} not found", request.SaleId);
                    return CommandResult<bool>.CreateFailure("Sale not found.");
                }

                var item = sale.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
                if (item == null)
                {
                    _logger.LogWarning("[UpdateSaleItemCommandHandler] Item with product {ProductId} not found in sale {SaleId}",
                        request.ProductId, request.SaleId);
                    return CommandResult<bool>.CreateFailure("Sale item not found.");
                }

                _logger.LogInformation("[UpdateSaleItemCommandHandler] Updating item quantity from {OldQuantity} to {NewQuantity}",
                    item.Quantity, request.Quantity);

                item.UpdateQuantity(request.Quantity);
                await _saleRepository.UpdateAsync(sale, cancellationToken);

                return CommandResult<bool>.CreateSuccess(true);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning("[UpdateSaleItemCommandHandler] Domain validation error: {Error}", ex.Message);
                return CommandResult<bool>.CreateFailure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UpdateSaleItemCommandHandler] Error updating item quantity");
                return CommandResult<bool>.CreateFailure("An unexpected error occurred while updating the item quantity.");
            }
        }
    }
}