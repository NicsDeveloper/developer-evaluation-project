using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(x => x.SaleNumber)
            .NotEmpty().WithMessage("Sale number is required")
            .MaximumLength(50).WithMessage("Sale number cannot exceed 50 characters");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MaximumLength(200).WithMessage("Customer name cannot exceed 200 characters");

        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("Branch ID is required");

        RuleFor(x => x.BranchName)
            .NotEmpty().WithMessage("Branch name is required")
            .MaximumLength(200).WithMessage("Branch name cannot exceed 200 characters");

        RuleFor(x => x.Date)
            .Must(date => !date.HasValue || date.Value <= DateTime.UtcNow)
            .WithMessage("Sale date cannot be in the future");

        RuleFor(x => x.Items)
            .Must(items => items.Count <= 100)
            .WithMessage("Sale cannot have more than 100 items");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateSaleItemRequestValidator());
    }
}

public class CreateSaleItemRequestValidator : AbstractValidator<CreateSaleItemRequest>
{
    public CreateSaleItemRequestValidator()
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
