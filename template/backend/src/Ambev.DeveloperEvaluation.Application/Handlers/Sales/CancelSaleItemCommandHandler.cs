using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Commands.Sales;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Handlers.Sales
{
    public class CancelSaleItemCommandHandler : IRequestHandler<CancelSaleItemCommand, CommandResult<bool>>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelSaleItemCommandHandler(ISaleRepository saleRepository, IUnitOfWork unitOfWork)
        {
            _saleRepository = saleRepository;
            _unitOfWork = unitOfWork;
        }

        public Task<CommandResult<bool>> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommandResult<bool>.CreateFailure("Cancelling individual items is no longer supported. Please cancel the entire sale instead."));
        }
    }
}