using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories
{
  public interface ISaleRepository
  {
    // Basic CRUD operations
    Task<Sale> CreateAsync(Sale sale);
    Task<Sale?> GetByIdAsync(Guid id);
    Task<Sale?> GetBySaleNumberAsync(string saleNumber);
    Task<IEnumerable<Sale>> GetAllAsync();
    Task<Sale> UpdateAsync(Sale sale);
    Task DeleteAsync(Guid id);

    // Business specific queries
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<Sale>> GetByBranchIdAsync(Guid branchId);
    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Sale>> GetActiveSalesAsync();
    Task<IEnumerable<Sale>> GetCancelledSalesAsync();

    // Aggregations
    Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalSalesByCustomerAsync(Guid customerId);
    Task<decimal> GetTotalSalesByBranchAsync(Guid branchId);

    // Existence checks
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsBySaleNumberAsync(string saleNumber);
  }
}
