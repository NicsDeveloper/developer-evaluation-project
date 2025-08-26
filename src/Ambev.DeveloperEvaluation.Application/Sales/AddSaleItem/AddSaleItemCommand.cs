using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem;

public record AddSaleItemCommand : IRequest<AddSaleItemResult>
{
  public Guid SaleId { get; init; }
  public Guid ProductId { get; init; }
  public string ProductName { get; init; } = string.Empty;
  public int Quantity { get; init; }
  public decimal UnitPrice { get; init; }
}
