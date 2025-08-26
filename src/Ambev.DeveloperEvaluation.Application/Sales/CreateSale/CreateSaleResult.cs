namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public record CreateSaleResult
{
  public Guid Id { get; init; }
  public string SaleNumber { get; init; } = string.Empty;
  public DateTime Date { get; init; }
  public Guid CustomerId { get; init; }
  public string CustomerName { get; init; } = string.Empty;
  public Guid BranchId { get; init; }
  public string BranchName { get; init; } = string.Empty;
  public decimal GrossTotal { get; init; }
  public decimal DiscountTotal { get; init; }
  public decimal NetTotal { get; init; }
  public bool Cancelled { get; init; }
  public DateTime CreatedAt { get; init; }
  public List<CreateSaleItemResult> Items { get; init; } = new();
}

public record CreateSaleItemResult
{
  public Guid Id { get; init; }
  public Guid ProductId { get; init; }
  public string ProductName { get; init; } = string.Empty;
  public int Quantity { get; init; }
  public decimal UnitPrice { get; init; }
  public decimal DiscountPercent { get; init; }
  public decimal DiscountAmount { get; init; }
  public decimal GrossAmount { get; init; }
  public decimal NetAmount { get; init; }
  public bool Cancelled { get; init; }
}
