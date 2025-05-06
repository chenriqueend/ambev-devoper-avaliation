using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Commands.Customers
{
    public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CommandResult<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await _customerRepository.EmailExistsAsync(request.Email, cancellationToken))
                {
                    return CommandResult<Guid>.CreateFailure("A customer with this email already exists.");
                }

                var customer = new Customer(request.Name, request.Email, request.Phone);

                await _customerRepository.AddAsync(customer);
                await _unitOfWork.Commit();

                return CommandResult<Guid>.CreateSuccess(customer.Id);
            }
            catch (Exception ex)
            {
                return CommandResult<Guid>.CreateFailure(ex.Message);
            }
        }
    }
}