using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales
{
  public record GetSalesQuery : IRequest<GetSalesResult>
  {
    public Guid? CustomerId { get; init; }
    public Guid? BranchId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool? Cancelled { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
  }
}
