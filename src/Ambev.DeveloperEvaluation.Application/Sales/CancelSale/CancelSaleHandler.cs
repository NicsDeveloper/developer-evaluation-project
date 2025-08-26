using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;

    public CancelSaleHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<CancelSaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id) 
            ?? throw new InvalidOperationException($"Sale with ID {request.Id} not found");

        if (sale.Cancelled)
        {
            throw new InvalidOperationException($"Sale {sale.SaleNumber} is already cancelled");
        }

        sale.Cancel();

        var updatedSale = await _saleRepository.UpdateAsync(sale);

        return new CancelSaleResult
        {
            Id = updatedSale.Id,
            SaleNumber = updatedSale.SaleNumber,
            Cancelled = updatedSale.Cancelled,
            UpdatedAt = updatedSale.UpdatedAt
        };
    }
}
