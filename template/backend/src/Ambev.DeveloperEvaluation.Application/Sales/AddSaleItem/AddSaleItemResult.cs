namespace Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem
{
  public record AddSaleItemResult
  {
    public Guid SaleId { get; init; }
    public Guid ItemId { get; init; }
    public decimal NewGrossTotal { get; init; }
    public decimal NewDiscountTotal { get; init; }
    public decimal NewNetTotal { get; init; }
    public int TotalItems { get; init; }
  }
}
