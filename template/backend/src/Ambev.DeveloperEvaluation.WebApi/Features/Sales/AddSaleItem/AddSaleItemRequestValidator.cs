using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddSaleItem;

public class AddSaleItemRequestValidator : AbstractValidator<AddSaleItemRequest>
{
    public AddSaleItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(20).WithMessage("Quantity cannot exceed 20 items");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative");
    }
}
