using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem
{
  public class AddSaleItemValidator : AbstractValidator<AddSaleItemCommand>
  {
    public AddSaleItemValidator()
    {
      _ = RuleFor(static x => x.SaleId)
          .NotEmpty().WithMessage("Sale ID is required");

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
