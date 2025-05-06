using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Commands.Branches
{
    public class CreateBranchCommandHandler : ICommandHandler<CreateBranchCommand, Guid>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateBranchCommandHandler(IBranchRepository branchRepository, IUnitOfWork unitOfWork)
        {
            _branchRepository = branchRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CommandResult<Guid>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var branch = new Branch(request.Name, request.Address, request.Phone);

                await _branchRepository.AddAsync(branch);
                await _unitOfWork.Commit();

                return CommandResult<Guid>.CreateSuccess(branch.Id);
            }
            catch (Exception ex)
            {
                return CommandResult<Guid>.CreateFailure(ex.Message);
            }
        }
    }
}