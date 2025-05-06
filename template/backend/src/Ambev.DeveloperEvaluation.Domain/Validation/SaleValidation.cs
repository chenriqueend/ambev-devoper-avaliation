using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Validation
{
    public class SaleValidation : AbstractValidator<Sale>
    {
        public SaleValidation()
        {
            RuleFor(s => s.SaleNumber)
                .NotEmpty()
                .WithMessage("Sale number is required")
                .MaximumLength(50)
                .WithMessage("Sale number must not exceed 50 characters");

            RuleFor(s => s.CustomerId)
                .NotEmpty()
                .WithMessage("Customer ID is required");

            RuleFor(s => s.CustomerName)
                .NotEmpty()
                .WithMessage("Customer name is required")
                .MaximumLength(200)
                .WithMessage("Customer name must not exceed 200 characters");

            RuleFor(s => s.BranchId)
                .NotEmpty()
                .WithMessage("Branch ID is required");

            RuleFor(s => s.BranchName)
                .NotEmpty()
                .WithMessage("Branch name is required")
                .MaximumLength(200)
                .WithMessage("Branch name must not exceed 200 characters");

            RuleFor(s => s.TotalWithDiscount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Total with discount must be greater than or equal to 0");
        }
    }
}