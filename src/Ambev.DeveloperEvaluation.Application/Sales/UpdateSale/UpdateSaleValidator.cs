using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
  public class UpdateSaleValidator : AbstractValidator<UpdateSaleCommand>
  {
    public UpdateSaleValidator()
    {
      _ = RuleFor(static x => x.Id)
          .NotEmpty().WithMessage("Sale ID is required");

      _ = RuleFor(static x => x.CustomerName)
          .MaximumLength(200).WithMessage("Customer name cannot exceed 200 characters")
          .When(static x => !string.IsNullOrEmpty(x.CustomerName));

      _ = RuleFor(static x => x.BranchName)
          .MaximumLength(200).WithMessage("Branch name cannot exceed 200 characters")
          .When(static x => !string.IsNullOrEmpty(x.BranchName));

      _ = RuleFor(static x => x)
          .Must(static x => !string.IsNullOrEmpty(x.CustomerName) || !string.IsNullOrEmpty(x.BranchName))
          .WithMessage("At least one field must be provided for update");
    }
  }
}
