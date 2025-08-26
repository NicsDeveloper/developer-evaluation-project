using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem
{
  public class AddSaleItemHandler : IRequestHandler<AddSaleItemCommand, AddSaleItemResult>
  {
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<AddSaleItemHandler> _logger;

    public AddSaleItemHandler(ISaleRepository saleRepository, ILogger<AddSaleItemHandler> logger)
    {
      _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AddSaleItemResult> Handle(AddSaleItemCommand request, CancellationToken cancellationToken)
    {
      try
      {
        _logger.LogInformation("Adding item to sale with ID: {SaleId}", request.SaleId);

        Sale sale = await _saleRepository.GetByIdAsync(request.SaleId)
                  ?? throw new InvalidOperationException($"Sale with ID {request.SaleId} not found");

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

        _logger.LogInformation("Item {ItemId} successfully added to sale {SaleNumber}",
            item.Id, sale.SaleNumber);

        return new AddSaleItemResult
        {
          SaleId = sale.Id,
          ItemId = item.Id,
          NewGrossTotal = sale.GrossTotal,
          NewDiscountTotal = sale.DiscountTotal,
          NewNetTotal = sale.NetTotal,
          TotalItems = sale.Items.Count
        };
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error adding item to sale with ID: {SaleId}", request.SaleId);
        throw;
      }
    }
  }
}
