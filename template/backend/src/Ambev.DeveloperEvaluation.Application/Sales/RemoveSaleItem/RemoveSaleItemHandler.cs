using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem
{
  public class RemoveSaleItemHandler : IRequestHandler<RemoveSaleItemCommand, RemoveSaleItemResult>
  {
    private readonly ISaleRepository _saleRepository;

    public RemoveSaleItemHandler(ISaleRepository saleRepository)
    {
      _saleRepository = saleRepository;
    }

    public async Task<RemoveSaleItemResult> Handle(RemoveSaleItemCommand request, CancellationToken cancellationToken)
    {
      Domain.Entities.Sale sale = await _saleRepository.GetByIdAsync(request.SaleId) ?? throw new InvalidOperationException($"Sale with ID {request.SaleId} not found");
      if (sale.Cancelled)
      {
        throw new InvalidOperationException($"Cannot remove items from cancelled sale {sale.SaleNumber}");
      }

      Domain.Entities.SaleItem item = sale.Items.FirstOrDefault(i => i.Id == request.ItemId) ?? throw new InvalidOperationException($"Item with ID {request.ItemId} not found in sale {sale.SaleNumber}");

      sale.RemoveItem(request.ItemId);

      Domain.Entities.Sale updatedSale = await _saleRepository.UpdateAsync(sale);

      return new RemoveSaleItemResult
      {
        SaleId = updatedSale.Id,
        RemovedItemId = request.ItemId,
        NewGrossTotal = updatedSale.GrossTotal,
        NewDiscountTotal = updatedSale.DiscountTotal,
        NewNetTotal = updatedSale.NetTotal,
        TotalItems = updatedSale.Items.Count
      };
    }
  }
}
