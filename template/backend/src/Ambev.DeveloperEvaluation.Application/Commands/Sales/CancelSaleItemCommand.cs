using System;
using Ambev.DeveloperEvaluation.Application.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Commands.Sales
{
    public class CancelSaleItemCommand : IRequest<CommandResult<bool>>
    {
        public Guid SaleId { get; }
        public Guid ProductId { get; }

        public CancelSaleItemCommand(Guid saleId, Guid productId)
        {
            SaleId = saleId;
            ProductId = productId;
        }
    }
}