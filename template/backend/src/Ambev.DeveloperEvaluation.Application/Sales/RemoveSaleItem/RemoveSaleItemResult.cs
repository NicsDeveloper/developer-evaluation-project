namespace Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem
{
  public record RemoveSaleItemResult
  {
    public Guid SaleId { get; init; }
    public Guid RemovedItemId { get; init; }
    public decimal NewGrossTotal { get; init; }
    public decimal NewDiscountTotal { get; init; }
    public decimal NewNetTotal { get; init; }
    public int TotalItems { get; init; }
  }
}
