namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale
{
  public record CancelSaleResult
  {
    public Guid Id { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public bool Cancelled { get; init; }
    public DateTime? UpdatedAt { get; init; }
  }
}
