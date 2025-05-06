using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Commands.Sales;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Handlers.Sales
{
    public class CancelSaleCommandHandler : ICommandHandler<CancelSaleCommand, bool>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CancelSaleCommandHandler> _logger;

        public CancelSaleCommandHandler(
            ISaleRepository saleRepository,
            IUnitOfWork unitOfWork,
            ILogger<CancelSaleCommandHandler> logger)
        {
            _saleRepository = saleRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CommandResult<bool>> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("[CancelSaleCommandHandler] Starting to handle cancel request for sale {SaleId}", request.SaleId);

                // Try to cancel the sale
                _logger.LogInformation("[CancelSaleCommandHandler] Attempting to cancel sale {SaleId}", request.SaleId);
                var success = await _saleRepository.CancelSaleAsync(request.SaleId, cancellationToken);

                if (!success)
                {
                    _logger.LogWarning("[CancelSaleCommandHandler] Failed to cancel sale {SaleId}", request.SaleId);
                    return CommandResult<bool>.CreateFailure("Failed to cancel sale. The sale may not exist or is already cancelled.");
                }

                _logger.LogInformation("[CancelSaleCommandHandler] Successfully cancelled sale {SaleId}", request.SaleId);

                // Commit the transaction
                _logger.LogInformation("[CancelSaleCommandHandler] Committing transaction");
                var commitResult = await _unitOfWork.Commit();

                if (!commitResult)
                {
                    _logger.LogWarning("[CancelSaleCommandHandler] Failed to commit transaction for sale {SaleId}, but sale was cancelled", request.SaleId);
                    // Return success anyway since the sale was actually cancelled
                    return CommandResult<bool>.CreateSuccess(true);
                }

                _logger.LogInformation("[CancelSaleCommandHandler] Transaction committed successfully");

                return CommandResult<bool>.CreateSuccess(true);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "[CancelSaleCommandHandler] Concurrency conflict while cancelling sale {SaleId}", request.SaleId);
                return CommandResult<bool>.CreateFailure("The sale was modified by another operation. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CancelSaleCommandHandler] Error cancelling sale {SaleId}", request.SaleId);
                return CommandResult<bool>.CreateFailure($"An error occurred while cancelling the sale: {ex.Message}");
            }
        }
    }
}