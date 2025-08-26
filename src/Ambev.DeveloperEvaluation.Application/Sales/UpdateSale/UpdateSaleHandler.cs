using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
  public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
  {
    private readonly ISaleRepository _saleRepository;

    public UpdateSaleHandler(ISaleRepository saleRepository)
    {
      _saleRepository = saleRepository;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
      if (string.IsNullOrWhiteSpace(request.CustomerName) && string.IsNullOrWhiteSpace(request.BranchName))
      {
        throw new InvalidOperationException("At least one field must be provided for update");
      }

      Domain.Entities.Sale sale = await _saleRepository.GetByIdAsync(request.Id)
              ?? throw new InvalidOperationException($"Sale with ID {request.Id} not found");

      if (sale.Cancelled)
      {
        throw new InvalidOperationException($"Cannot update cancelled sale {sale.SaleNumber}");
      }

      sale.Update(
          request.CustomerName ?? sale.CustomerName,
          request.BranchName ?? sale.BranchName
      );

      Domain.Entities.Sale updatedSale = await _saleRepository.UpdateAsync(sale);

      return new UpdateSaleResult
      {
        Id = updatedSale.Id,
        SaleNumber = updatedSale.SaleNumber,
        CustomerName = updatedSale.CustomerName,
        BranchName = updatedSale.BranchName,
        UpdatedAt = updatedSale.UpdatedAt
      };
    }
  }
}
