using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem;

public class RemoveSaleItemValidator : AbstractValidator<RemoveSaleItemCommand>
{
  public RemoveSaleItemValidator()
  {
    RuleFor(x => x.SaleId)
        .NotEmpty().WithMessage("Sale ID is required");

    RuleFor(x => x.ItemId)
        .NotEmpty().WithMessage("Item ID is required");
  }
}
