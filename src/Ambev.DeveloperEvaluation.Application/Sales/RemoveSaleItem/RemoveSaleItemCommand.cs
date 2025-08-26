using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem
{
  public record RemoveSaleItemCommand : IRequest<RemoveSaleItemResult>
  {
    public Guid SaleId { get; init; }
    public Guid ItemId { get; init; }
  }
}
