using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public record CreateSaleCommand : IRequest<CreateSaleResult>
{
  public string SaleNumber { get; init; } = string.Empty;
  public Guid CustomerId { get; init; }
  public string CustomerName { get; init; } = string.Empty;
  public Guid BranchId { get; init; }
  public string BranchName { get; init; } = string.Empty;
  public DateTime? Date { get; init; }
  public List<CreateSaleItemRequest> Items { get; init; } = new();
}

public record CreateSaleItemRequest
{
  public Guid ProductId { get; init; }
  public string ProductName { get; init; } = string.Empty;
  public int Quantity { get; init; }
  public decimal UnitPrice { get; init; }
}
