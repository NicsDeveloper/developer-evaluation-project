using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleHandler : IRequestHandler<GetSaleQuery, GetSaleResult?>
{
  private readonly ISaleRepository _saleRepository;

  public GetSaleHandler(ISaleRepository saleRepository)
  {
    _saleRepository = saleRepository;
  }

  public async Task<GetSaleResult?> Handle(GetSaleQuery request, CancellationToken cancellationToken)
  {
    var sale = await _saleRepository.GetByIdAsync(request.Id);

    if (sale == null)
      return null;

    return new GetSaleResult
    {
      Id = sale.Id,
      SaleNumber = sale.SaleNumber,
      Date = sale.Date,
      CustomerId = sale.CustomerId,
      CustomerName = sale.CustomerName,
      BranchId = sale.BranchId,
      BranchName = sale.BranchName,
      GrossTotal = sale.GrossTotal,
      DiscountTotal = sale.DiscountTotal,
      NetTotal = sale.NetTotal,
      Cancelled = sale.Cancelled,
      CreatedAt = sale.CreatedAt,
      UpdatedAt = sale.UpdatedAt,
      Items = sale.Items.Select(i => new GetSaleItemResult
      {
        Id = i.Id,
        ProductId = i.ProductId,
        ProductName = i.ProductName,
        Quantity = i.Quantity,
        UnitPrice = i.UnitPrice,
        DiscountPercent = i.DiscountPercent,
        DiscountAmount = i.DiscountAmount,
        GrossAmount = i.GrossAmount,
        NetAmount = i.NetAmount,
        Cancelled = i.Cancelled,
        CreatedAt = i.CreatedAt,
        UpdatedAt = i.UpdatedAt
      }).ToList()
    };
  }
}
