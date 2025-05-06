using System;
using Ambev.DeveloperEvaluation.Application.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Commands.Sales
{
    public class CancelSaleCommand : IRequest<CommandResult<bool>>
    {
        public Guid SaleId { get; }

        public CancelSaleCommand(Guid saleId)
        {
            SaleId = saleId;
        }
    }
}