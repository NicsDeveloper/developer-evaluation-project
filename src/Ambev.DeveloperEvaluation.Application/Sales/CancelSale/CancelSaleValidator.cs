using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale
{
  public class CancelSaleValidator : AbstractValidator<CancelSaleCommand>
  {
    public CancelSaleValidator()
    {
      _ = RuleFor(static x => x.Id)
          .NotEmpty().WithMessage("Sale ID is required");
    }
  }
}
