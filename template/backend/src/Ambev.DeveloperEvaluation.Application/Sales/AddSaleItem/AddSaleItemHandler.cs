using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem
{
  public class AddSaleItemHandler : IRequestHandler<AddSaleItemCommand, AddSaleItemResult>
  {
    private readonly ISaleRepository _saleRepository;

    public AddSaleItemHandler(ISaleRepository saleRepository)
    {
      _saleRepository = saleRepository;
    }

    public async Task<AddSaleItemResult> Handle(AddSaleItemCommand request, CancellationToken cancellationToken)
    {
      Sale sale = await _saleRepository.GetByIdAsync(request.SaleId) ?? throw new InvalidOperationException($"Sale with ID {request.SaleId} not found");
      if (sale.Cancelled)
      {
        throw new InvalidOperationException($"Cannot add items to cancelled sale {sale.SaleNumber}");
      }

      if (sale.Items.Count >= 100)
      {
        throw new InvalidOperationException($"Sale {sale.SaleNumber} already has maximum number of items (100)");
      }

      SaleItem item = SaleItem.Create(
              request.ProductId,
              request.ProductName,
              request.Quantity,
              request.UnitPrice,
              sale.Id
          );

      sale.AddItem(item);

      Sale updatedSale = await _saleRepository.UpdateAsync(sale);

      return new AddSaleItemResult
      {
        SaleId = updatedSale.Id,
        ItemId = item.Id,
        NewGrossTotal = updatedSale.GrossTotal,
        NewDiscountTotal = updatedSale.DiscountTotal,
        NewNetTotal = updatedSale.NetTotal,
        TotalItems = updatedSale.Items.Count
      };
    }
  }
}
