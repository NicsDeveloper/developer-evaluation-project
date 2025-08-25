namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
  public record UpdateSaleResult
  {
    public Guid Id { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string BranchName { get; init; } = string.Empty;
    public DateTime? UpdatedAt { get; init; }
  }
}
