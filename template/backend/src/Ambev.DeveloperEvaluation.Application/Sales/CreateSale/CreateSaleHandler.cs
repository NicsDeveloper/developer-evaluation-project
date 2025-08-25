using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
  public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
  {
    private readonly ISaleRepository _saleRepository;

    public CreateSaleHandler(ISaleRepository saleRepository)
    {
      _saleRepository = saleRepository;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
      Sale? existingSale = await _saleRepository.GetBySaleNumberAsync(request.SaleNumber);
      if (existingSale != null)
      {
        throw new InvalidOperationException($"Sale with number {request.SaleNumber} already exists");
      }

      Sale sale = Sale.Create(
              request.SaleNumber,
              request.CustomerId,
              request.CustomerName,
              request.BranchId,
              request.BranchName
          );

      if (request.Date.HasValue)
      {
        sale.SetDate(request.Date.Value);
      }

      foreach (CreateSaleItemRequest itemRequest in request.Items)
      {
        SaleItem item = SaleItem.Create(
                  itemRequest.ProductId,
                  itemRequest.ProductName,
                  itemRequest.Quantity,
                  itemRequest.UnitPrice,
                  sale.Id
              );
        sale.AddItem(item);
      }

      Sale createdSale = await _saleRepository.CreateAsync(sale);

      return new CreateSaleResult
      {
        Id = createdSale.Id,
        SaleNumber = createdSale.SaleNumber,
        Date = createdSale.Date,
        CustomerId = createdSale.CustomerId,
        CustomerName = createdSale.CustomerName,
        BranchId = createdSale.BranchId,
        BranchName = createdSale.BranchName,
        GrossTotal = createdSale.GrossTotal,
        DiscountTotal = createdSale.DiscountTotal,
        NetTotal = createdSale.NetTotal,
        Cancelled = createdSale.Cancelled,
        CreatedAt = createdSale.CreatedAt,
        Items = createdSale.Items.Select(static i => new CreateSaleItemResult
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
          Cancelled = i.Cancelled
        }).ToList()
      };
    }
  }
}
