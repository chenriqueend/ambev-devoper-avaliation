using System;
using Ambev.DeveloperEvaluation.Application.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Commands.Sales
{
    public class UpdateSaleItemCommand : IRequest<CommandResult<bool>>
    {
        public Guid SaleId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }

        public UpdateSaleItemCommand(Guid saleId, Guid productId, int quantity)
        {
            SaleId = saleId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}