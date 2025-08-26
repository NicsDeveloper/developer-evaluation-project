using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem;

public class RemoveSaleItemHandler : IRequestHandler<RemoveSaleItemCommand, RemoveSaleItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<RemoveSaleItemHandler> _logger;

    public RemoveSaleItemHandler(ISaleRepository saleRepository, ILogger<RemoveSaleItemHandler> logger)
    {
        _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RemoveSaleItemResult> Handle(RemoveSaleItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Removing item {ItemId} from sale {SaleId}", request.ItemId, request.SaleId);

            var sale = await _saleRepository.GetByIdAsync(request.SaleId);
            if (sale == null)
            {
                _logger.LogWarning("Sale with ID {SaleId} not found", request.SaleId);
                throw new InvalidOperationException($"Venda com ID {request.SaleId} não encontrada");
            }

            if (sale.Cancelled)
            {
                _logger.LogWarning("Cannot remove items from cancelled sale {SaleNumber}", sale.SaleNumber);
                throw new InvalidOperationException($"Não é possível remover itens de uma venda cancelada {sale.SaleNumber}");
            }

            var item = sale.Items.FirstOrDefault(i => i.Id == request.ItemId);
            if (item == null)
            {
                _logger.LogWarning("Item with ID {ItemId} not found in sale {SaleNumber}", request.ItemId, sale.SaleNumber);
                throw new InvalidOperationException($"Item com ID {request.ItemId} não encontrado na venda {sale.SaleNumber}");
            }

            if (item.Cancelled)
            {
                _logger.LogWarning("Cannot remove cancelled item {ItemId} from sale {SaleNumber}", request.ItemId, sale.SaleNumber);
                throw new InvalidOperationException($"Não é possível remover um item cancelado {request.ItemId} da venda {sale.SaleNumber}");
            }

            sale.RemoveItem(request.ItemId);

            var updatedSale = await _saleRepository.UpdateAsync(sale);

            _logger.LogInformation("Item {ItemId} successfully removed from sale {SaleNumber}", request.ItemId, sale.SaleNumber);

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
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Unexpected error removing item {ItemId} from sale {SaleId}", request.ItemId, request.SaleId);
            throw new InvalidOperationException($"Erro inesperado ao remover item: {ex.Message}");
        }
    }
}
