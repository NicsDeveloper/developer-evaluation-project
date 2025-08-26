using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale)
    {
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber);
    }

    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        return await _context.Sales
            .Include(s => s.Items)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<Sale> UpdateAsync(Sale sale)
    {
        // Buscar a entidade existente para evitar problemas de tracking
        var existingSale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == sale.Id);

        if (existingSale == null)
        {
            throw new InvalidOperationException($"Sale with ID {sale.Id} not found");
        }

        // Atualizar as propriedades da entidade existente
        _context.Entry(existingSale).CurrentValues.SetValues(sale);
        
        // Atualizar os itens
        foreach (var item in sale.Items)
        {
            var existingItem = existingSale.Items.FirstOrDefault(i => i.Id == item.Id);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(item);
            }
            else
            {
                // Novo item
                existingSale.Items.Add(item);
            }
        }

        // Remover itens que não estão mais na lista
        var itemsToRemove = existingSale.Items
            .Where(existingItem => !sale.Items.Any(item => item.Id == existingItem.Id))
            .ToList();

        foreach (var itemToRemove in itemsToRemove)
        {
            existingSale.Items.Remove(itemToRemove);
            _context.SaleItems.Remove(itemToRemove);
        }

        await _context.SaveChangesAsync();
        return existingSale;
    }

    public async Task DeleteAsync(Guid id)
    {
        var sale = await GetByIdAsync(id);
        if (sale != null)
        {
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByBranchIdAsync(Guid branchId)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.BranchId == branchId)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.Date >= startDate && s.Date <= endDate)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetActiveSalesAsync()
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => !s.Cancelled)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetCancelledSalesAsync()
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.Cancelled)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Where(s => s.Date >= startDate && s.Date <= endDate && !s.Cancelled)
            .SumAsync(s => s.NetTotal);
    }

    public async Task<decimal> GetTotalSalesByCustomerAsync(Guid customerId)
    {
        return await _context.Sales
            .Where(s => s.CustomerId == customerId && !s.Cancelled)
            .SumAsync(s => s.NetTotal);
    }

    public async Task<decimal> GetTotalSalesByBranchAsync(Guid branchId)
    {
        return await _context.Sales
            .Where(s => s.BranchId == branchId && !s.Cancelled)
            .SumAsync(s => s.NetTotal);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Sales.AnyAsync(s => s.Id == id);
    }

    public async Task<bool> ExistsBySaleNumberAsync(string saleNumber)
    {
        return await _context.Sales.AnyAsync(s => s.SaleNumber == saleNumber);
    }
}
