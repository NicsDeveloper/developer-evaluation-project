using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
  public class GetSaleValidator : AbstractValidator<GetSaleQuery>
  {
    public GetSaleValidator()
    {
      _ = RuleFor(static x => x.Id)
          .NotEmpty().WithMessage("Sale ID is required");
    }
  }
}
