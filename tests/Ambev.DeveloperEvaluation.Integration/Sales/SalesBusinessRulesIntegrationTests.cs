using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using WebApiCreateSale = Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using WebApiUpdateSale = Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales;

public class SalesBusinessRulesIntegrationTests : IDisposable
{
  private readonly TestServer _testServer;
  private readonly HttpClient _client;
  private readonly DefaultContext _context;

  public SalesBusinessRulesIntegrationTests()
  {
    var builder = new WebHostBuilder()
        .UseStartup<TestStartup>();

    _testServer = new TestServer(builder);
    _client = _testServer.CreateClient();

    var scope = _testServer.Services.CreateScope();
    _context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
    _context.Database.EnsureCreated();
  }

  public void Dispose()
  {
    _client?.Dispose();
    _testServer?.Dispose();
  }

  [Fact]
  public async Task CreateSale_WithDuplicateSaleNumber_ShouldReturnBadRequest()
  {
    // Arrange - Create first sale
    var sale1 = Sale.Create(
        "DUPLICATE-001",
        Guid.NewGuid(),
        "Cliente 1",
        Guid.NewGuid(),
        "Filial 1"
    );
    _context.Sales.Add(sale1);
    await _context.SaveChangesAsync();

    // Try to create second sale with same number
    var request = new WebApiCreateSale.CreateSaleRequest
    {
      SaleNumber = "DUPLICATE-001", // Same number
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente 2",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial 2",
      Items = new List<WebApiCreateSale.CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto",
                    Quantity = 1,
                    UnitPrice = 10.00m
                }
            }
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/sales", request);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task CreateSale_WithQuantityExceedingMaxPerProduct_ShouldReturnBadRequest()
  {
    // Arrange
    var request = new WebApiCreateSale.CreateSaleRequest
    {
      SaleNumber = "QUANTITY-TEST",
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente Quantidade",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial Quantidade",
      Items = new List<WebApiCreateSale.CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto A",
                    Quantity = 21, // Exceeds max 20
                    UnitPrice = 10.00m
                }
            }
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/sales", request);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task CreateSale_WithNegativeUnitPrice_ShouldReturnBadRequest()
  {
    // Arrange
    var request = new WebApiCreateSale.CreateSaleRequest
    {
      SaleNumber = "PRICE-TEST",
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente Preço",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial Preço",
      Items = new List<WebApiCreateSale.CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto Negativo",
                    Quantity = 1,
                    UnitPrice = -5.00m // Negative price
                }
            }
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/sales", request);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task CreateSale_WithFutureDate_ShouldReturnBadRequest()
  {
    // Arrange
    var request = new WebApiCreateSale.CreateSaleRequest
    {
      SaleNumber = "DATE-TEST",
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente Data",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial Data",
      Date = DateTime.UtcNow.AddDays(1), // Future date
      Items = new List<WebApiCreateSale.CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto Data",
                    Quantity = 1,
                    UnitPrice = 10.00m
                }
            }
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/sales", request);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task CreateSale_WithEmptyItems_ShouldReturnBadRequest()
  {
    // Arrange
    var request = new WebApiCreateSale.CreateSaleRequest
    {
      SaleNumber = "EMPTY-ITEMS",
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente Vazio",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial Vazio",
      Items = new List<WebApiCreateSale.CreateSaleItemRequest>() // Empty items
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/sales", request);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task CreateSale_WithTooManyItems_ShouldReturnBadRequest()
  {
    // Arrange
    var items = new List<WebApiCreateSale.CreateSaleItemRequest>();
    for (int i = 0; i < 101; i++) // 101 items (exceeds max 100)
    {
      items.Add(new WebApiCreateSale.CreateSaleItemRequest
      {
        ProductId = Guid.NewGuid(),
        ProductName = $"Produto {i}",
        Quantity = 1,
        UnitPrice = 10.00m
      });
    }

    var request = new WebApiCreateSale.CreateSaleRequest
    {
      SaleNumber = "MANY-ITEMS",
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente Muitos Itens",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial Muitos Itens",
      Items = items
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/sales", request);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task CreateSale_WithValidDiscountCalculation_ShouldReturnCorrectTotals()
  {
    // Arrange - Create sale with quantity that should get discount
    var request = new WebApiCreateSale.CreateSaleRequest
    {
      SaleNumber = "DISCOUNT-TEST",
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente Desconto",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial Desconto",
      Items = new List<WebApiCreateSale.CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto Desconto",
                    Quantity = 10, // Should get 10% discount
                    UnitPrice = 100.00m
                }
            }
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/sales", request);
    var content = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<ApiResponseWithData<CreateSaleResult>>(content, new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    });

    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode); // Corrigido: Created (201) para criação de venda
    Assert.NotNull(result?.Data);

    var item = result.Data.Items.First();
    Assert.Equal(1000.00m, item.GrossAmount); // 10 * 100
    Assert.Equal(100.00m, item.DiscountAmount); // 10% of 1000
    Assert.Equal(900.00m, item.NetAmount); // 1000 - 100
  }

  [Fact]
  public async Task UpdateSale_WithEmptyRequest_ShouldReturnBadRequest()
  {
    // Arrange
    var sale = Sale.Create(
        "UPDATE-EMPTY",
        Guid.NewGuid(),
        "Cliente Original",
        Guid.NewGuid(),
        "Filial Original"
    );
    _context.Sales.Add(sale);
    await _context.SaveChangesAsync();

    var updateRequest = new WebApiUpdateSale.UpdateSaleRequest
    {
      CustomerName = null,
      BranchName = null
    };

    // Act
    var response = await _client.PutAsJsonAsync($"/api/sales/{sale.Id}", updateRequest);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task CancelSale_WithAlreadyCancelledSale_ShouldReturnBadRequest()
  {
    // Arrange
    var sale = Sale.Create(
        "CANCEL-TWICE",
        Guid.NewGuid(),
        "Cliente Cancelar",
        Guid.NewGuid(),
        "Filial Cancelar"
    );
    sale.Cancel(); // Cancel first time
    _context.Sales.Add(sale);
    await _context.SaveChangesAsync();

    // Act - Try to cancel again
    var response = await _client.DeleteAsync($"/api/sales/{sale.Id}");

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }
}
