using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
  public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
  {
    public CreateSaleValidator()
    {
      _ = RuleFor(static x => x.SaleNumber)
          .NotEmpty().WithMessage("Sale number is required")
          .MaximumLength(50).WithMessage("Sale number cannot exceed 50 characters");

      _ = RuleFor(static x => x.CustomerId)
          .NotEmpty().WithMessage("Customer ID is required");

      _ = RuleFor(static x => x.CustomerName)
          .NotEmpty().WithMessage("Customer name is required")
          .MaximumLength(200).WithMessage("Customer name cannot exceed 200 characters");

      _ = RuleFor(static x => x.BranchId)
          .NotEmpty().WithMessage("Branch ID is required");

      _ = RuleFor(static x => x.BranchName)
          .NotEmpty().WithMessage("Branch name is required")
          .MaximumLength(200).WithMessage("Branch name cannot exceed 200 characters");

      _ = RuleFor(static x => x.Date)
          .Must(static date => !date.HasValue || date.Value <= DateTime.UtcNow)
          .WithMessage("Sale date cannot be in the future");

      _ = RuleFor(static x => x.Items)
          .Must(static items => items.Count <= 100)
          .WithMessage("Sale cannot have more than 100 items");

      _ = RuleForEach(static x => x.Items)
          .SetValidator(new CreateSaleItemValidator());
    }
  }

  public class CreateSaleItemValidator : AbstractValidator<CreateSaleItemRequest>
  {
    public CreateSaleItemValidator()
    {
      _ = RuleFor(static x => x.ProductId)
          .NotEmpty().WithMessage("Product ID is required");

      _ = RuleFor(static x => x.ProductName)
          .NotEmpty().WithMessage("Product name is required")
          .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

      _ = RuleFor(static x => x.Quantity)
          .GreaterThan(0).WithMessage("Quantity must be greater than 0")
          .LessThanOrEqualTo(20).WithMessage("Quantity cannot exceed 20 items");

      _ = RuleFor(static x => x.UnitPrice)
          .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative");
    }
  }
}
