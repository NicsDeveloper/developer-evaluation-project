namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales
{
  public record GetSalesResult
  {
    public List<GetSalesItemResult> Sales { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
  }

  public record GetSalesItemResult
  {
    public Guid Id { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public Guid BranchId { get; init; }
    public string BranchName { get; init; } = string.Empty;
    public decimal NetTotal { get; init; }
    public bool Cancelled { get; init; }
    public DateTime CreatedAt { get; init; }
    public int ItemCount { get; init; }
  }
}
