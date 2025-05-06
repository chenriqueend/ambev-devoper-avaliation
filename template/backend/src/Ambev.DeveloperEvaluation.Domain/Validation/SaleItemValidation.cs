using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Validation
{
    public class SaleItemValidation : AbstractValidator<SaleItem>
    {
        public SaleItemValidation()
        {
            RuleFor(i => i.ProductId)
                .NotEmpty()
                .WithMessage("Product ID is required");

            RuleFor(i => i.ProductDescription)
                .NotEmpty()
                .WithMessage("Product description is required")
                .MaximumLength(200)
                .WithMessage("Product description must not exceed 200 characters");

            RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0");

            RuleFor(i => i.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than 0");

            RuleFor(i => i.Discount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Discount must be greater than or equal to 0");

            RuleFor(i => i.TotalAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Total amount must be greater than or equal to 0");
        }
    }
}