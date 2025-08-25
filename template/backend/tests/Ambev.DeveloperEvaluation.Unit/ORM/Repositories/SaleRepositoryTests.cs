using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.ORM.Repositories;

public class SaleRepositoryTests
{
  private readonly DbContextOptions<DefaultContext> _options;
  private readonly DefaultContext _context;
  private readonly ISaleRepository _repository;

  public SaleRepositoryTests()
  {
    _options = new DbContextOptionsBuilder<DefaultContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    _context = new DefaultContext(_options);
    _repository = new SaleRepository(_context);
  }

  [Fact]
  public async Task CreateAsync_ShouldCreateSaleSuccessfully()
  {
    // Arrange
    var sale = Sale.Create("SALE001", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");

    // Act
    var result = await _repository.CreateAsync(sale);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("SALE001", result.SaleNumber);
    Assert.NotEqual(Guid.Empty, result.Id);
  }

  [Fact]
  public async Task GetByIdAsync_WithExistingId_ShouldReturnSale()
  {
    // Arrange
    var sale = Sale.Create("SALE002", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    await _repository.CreateAsync(sale);

    // Act
    var result = await _repository.GetByIdAsync(sale.Id);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(sale.Id, result.Id);
    Assert.Equal("SALE002", result.SaleNumber);
  }

  [Fact]
  public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
  {
    // Arrange
    var nonExistingId = Guid.NewGuid();

    // Act
    var result = await _repository.GetByIdAsync(nonExistingId);

    // Assert
    Assert.Null(result);
  }

  [Fact]
  public async Task GetBySaleNumberAsync_WithExistingNumber_ShouldReturnSale()
  {
    // Arrange
    var sale = Sale.Create("SALE003", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    await _repository.CreateAsync(sale);

    // Act
    var result = await _repository.GetBySaleNumberAsync("SALE003");

    // Assert
    Assert.NotNull(result);
    Assert.Equal("SALE003", result.SaleNumber);
  }

  [Fact]
  public async Task GetAllAsync_ShouldReturnAllSales()
  {
    // Arrange
    var sale1 = Sale.Create("SALE004", Guid.NewGuid(), "Cliente 1", Guid.NewGuid(), "Filial 1");
    var sale2 = Sale.Create("SALE005", Guid.NewGuid(), "Cliente 2", Guid.NewGuid(), "Filial 2");
    await _repository.CreateAsync(sale1);
    await _repository.CreateAsync(sale2);

    // Act
    var result = await _repository.GetAllAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Equal(2, result.Count());
  }

  [Fact]
  public async Task UpdateAsync_ShouldUpdateSaleSuccessfully()
  {
    // Arrange
    var sale = Sale.Create("SALE006", Guid.NewGuid(), "Cliente Original", Guid.NewGuid(), "Filial Original");
    await _repository.CreateAsync(sale);
    sale.Update("Novo Cliente", "Nova Filial");

    // Act
    var result = await _repository.UpdateAsync(sale);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Novo Cliente", result.CustomerName);
    Assert.Equal("Nova Filial", result.BranchName);
  }

  [Fact]
  public async Task DeleteAsync_WithExistingId_ShouldDeleteSale()
  {
    // Arrange
    var sale = Sale.Create("SALE007", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    await _repository.CreateAsync(sale);

    // Act
    await _repository.DeleteAsync(sale.Id);

    // Assert
    var deletedSale = await _repository.GetByIdAsync(sale.Id);
    Assert.Null(deletedSale);
  }

  [Fact]
  public async Task GetByCustomerIdAsync_ShouldReturnCustomerSales()
  {
    // Arrange
    var customerId = Guid.NewGuid();
    var sale1 = Sale.Create("SALE008", customerId, "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    var sale2 = Sale.Create("SALE009", customerId, "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    var sale3 = Sale.Create("SALE010", Guid.NewGuid(), "Outro Cliente", Guid.NewGuid(), "Filial Teste");

    await _repository.CreateAsync(sale1);
    await _repository.CreateAsync(sale2);
    await _repository.CreateAsync(sale3);

    // Act
    var result = await _repository.GetByCustomerIdAsync(customerId);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(2, result.Count());
    Assert.All(result, s => Assert.Equal(customerId, s.CustomerId));
  }

  [Fact]
  public async Task GetByDateRangeAsync_ShouldReturnSalesInRange()
  {
    // Arrange
    var startDate = DateTime.Today.AddDays(-5);
    var endDate = DateTime.Today.AddDays(5);


    var sale1 = Sale.Create("SALE011", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    sale1.SetDate(DateTime.Today);

    var sale2 = Sale.Create("SALE012", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    sale2.SetDate(DateTime.Today.AddDays(-10)); // Outside range

    await _repository.CreateAsync(sale1);
    await _repository.CreateAsync(sale2);

    // Act
    var result = await _repository.GetByDateRangeAsync(startDate, endDate);

    // Assert
    Assert.NotNull(result);
    Assert.Single(result);
    Assert.Equal("SALE011", result.First().SaleNumber);
  }

  [Fact]
  public async Task GetActiveSalesAsync_ShouldReturnOnlyActiveSales()
  {
    // Arrange
    var activeSale = Sale.Create("SALE013", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    var cancelledSale = Sale.Create("SALE014", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    cancelledSale.Cancel();

    await _repository.CreateAsync(activeSale);
    await _repository.CreateAsync(cancelledSale);

    // Act
    var result = await _repository.GetActiveSalesAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Single(result);
    Assert.Equal("SALE013", result.First().SaleNumber);
    Assert.False(result.First().Cancelled);
  }

  [Fact]
  public async Task GetTotalSalesByDateRangeAsync_ShouldReturnCorrectTotal()
  {
    // Arrange
    var startDate = DateTime.Today.AddDays(-1);
    var endDate = DateTime.Today.AddDays(1);

    var sale1 = Sale.Create("SALE015", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    sale1.SetDate(DateTime.Today);
    var item1 = SaleItem.Create(Guid.NewGuid(), "Produto 1", 5, 10.00m, sale1.Id);
    sale1.AddItem(item1);

    var sale2 = Sale.Create("SALE016", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    sale2.SetDate(DateTime.Today);
    var item2 = SaleItem.Create(Guid.NewGuid(), "Produto 2", 10, 20.00m, sale2.Id);
    sale2.AddItem(item2);

    await _repository.CreateAsync(sale1);
    await _repository.CreateAsync(sale2);

    // Act
    var result = await _repository.GetTotalSalesByDateRangeAsync(startDate, endDate);

    // Assert
    Assert.Equal(227.50m, result); // (5 * 10 * 0.95) + (10 * 20 * 0.90)
  }

  [Fact]
  public async Task ExistsAsync_WithExistingId_ShouldReturnTrue()
  {
    // Arrange
    var sale = Sale.Create("SALE017", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    await _repository.CreateAsync(sale);

    // Act
    var result = await _repository.ExistsAsync(sale.Id);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public async Task ExistsAsync_WithNonExistingId_ShouldReturnFalse()
  {
    // Arrange
    var nonExistingId = Guid.NewGuid();

    // Act
    var result = await _repository.ExistsAsync(nonExistingId);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public async Task ExistsBySaleNumberAsync_WithExistingNumber_ShouldReturnTrue()
  {
    // Arrange
    var sale = Sale.Create("SALE018", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    await _repository.CreateAsync(sale);

    // Act
    var result = await _repository.ExistsBySaleNumberAsync("SALE018");

    // Assert
    Assert.True(result);
  }

  [Fact]
  public async Task ExistsBySaleNumberAsync_WithNonExistingNumber_ShouldReturnFalse()
  {
    // Arrange
    var nonExistingNumber = "NONEXISTENT";

    // Act
    var result = await _repository.ExistsBySaleNumberAsync(nonExistingNumber);

    // Assert
    Assert.False(result);
  }

  private void Dispose()
  {
    _context.Database.EnsureDeleted();
    _context.Dispose();
  }
}
