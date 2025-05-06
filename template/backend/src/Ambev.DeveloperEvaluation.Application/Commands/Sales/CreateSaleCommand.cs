using System;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Application.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Commands.Sales
{
    public class CreateSaleCommand : IRequest<CommandResult<Guid>>
    {
        public required string SaleNumber { get; set; }
        public required Guid CustomerId { get; set; }
        public required string CustomerName { get; set; }
        public required Guid BranchId { get; set; }
        public required string BranchName { get; set; }
        public required List<SaleItemDto> Items { get; set; }

        public class SaleItemDto
        {
            public required Guid ProductId { get; set; }
            public required string ProductDescription { get; set; }
            public required int Quantity { get; set; }
            public required decimal UnitPrice { get; set; }
        }
    }
}