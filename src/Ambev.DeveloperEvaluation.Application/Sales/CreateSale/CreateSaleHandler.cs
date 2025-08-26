using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;

    public CreateSaleHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var existingSale = await _saleRepository.GetBySaleNumberAsync(request.SaleNumber);
        if (existingSale != null)
        {
            throw new InvalidOperationException($"Sale with number {request.SaleNumber} already exists");
        }

        if (request.Date.HasValue && request.Date.Value > DateTime.UtcNow)
        {
            throw new InvalidOperationException("Cannot create sale with future date");
        }

        if (request.Items == null || request.Items.Count == 0)
        {
            throw new InvalidOperationException("Sale must have at least one item");
        }

        if (request.Items.Count > 100)
        {
            throw new InvalidOperationException("Sale cannot have more than 100 items");
        }

        var sale = Sale.Create(
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

        foreach (var itemRequest in request.Items)
        {
            var item = SaleItem.Create(
                itemRequest.ProductId,
                itemRequest.ProductName,
                itemRequest.Quantity,
                itemRequest.UnitPrice,
                sale.Id
            );
            sale.AddItem(item);
        }

        var createdSale = await _saleRepository.CreateAsync(sale);

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
            Items = createdSale.Items.Select(i => new CreateSaleItemResult
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
