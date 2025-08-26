using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales
{
  public class GetSalesValidator : AbstractValidator<GetSalesQuery>
  {
    public GetSalesValidator()
    {
      _ = RuleFor(static x => x.Page)
          .GreaterThan(0).WithMessage("Page must be greater than 0");

      _ = RuleFor(static x => x.PageSize)
          .GreaterThan(0).WithMessage("Page size must be greater than 0")
          .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

      _ = RuleFor(static x => x.StartDate)
          .Must(static (query, startDate) => !startDate.HasValue || !query.EndDate.HasValue || startDate <= query.EndDate)
          .WithMessage("Start date must be before or equal to end date");

      _ = RuleFor(static x => x.EndDate)
          .Must(static (query, endDate) => !endDate.HasValue || !query.StartDate.HasValue || endDate >= query.StartDate)
          .WithMessage("End date must be after or equal to start date");
    }
  }
}
