using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        RuleFor(x => x.CustomerName)
            .MaximumLength(200).WithMessage("Customer name cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.CustomerName));

        RuleFor(x => x.BranchName)
            .MaximumLength(200).WithMessage("Branch name cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.BranchName));

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.CustomerName) || !string.IsNullOrEmpty(x.BranchName))
            .WithMessage("At least one field must be provided for update");
    }
}
