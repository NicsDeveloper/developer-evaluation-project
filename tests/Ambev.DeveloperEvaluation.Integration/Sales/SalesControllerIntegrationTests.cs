using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;
using Xunit;

// Use aliases to resolve ambiguity
using WebApiCreateSale = Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using WebApiAddSaleItem = Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddSaleItem;
using WebApiUpdateSale = Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

namespace Ambev.DeveloperEvaluation.Integration.Sales;

/// <summary>
/// Testes de integração para o SalesController
/// </summary>
public class SalesControllerIntegrationTests : IDisposable
{
  private readonly TestServer _testServer;
  private readonly HttpClient _httpClient;
  private readonly List<Guid> _createdSaleIds = new();

  public SalesControllerIntegrationTests()
  {
    var builder = new WebHostBuilder()
        .UseStartup<TestStartup>();

    _testServer = new TestServer(builder);
    _httpClient = _testServer.CreateClient();
  }

  public void Dispose()
  {
    _httpClient?.Dispose();
    _testServer?.Dispose();
  }

  /// <summary>
  /// Testa a criação de uma venda com dados válidos
  /// </summary>
  [Fact]
  public async Task CreateSale_WithValidData_ShouldReturnSuccess()
  {
    // Arrange
    var request = new WebApiCreateSale.CreateSaleRequest
    {
      SaleNumber = "TEST-001",
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente Teste",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial Teste",
      Date = DateTime.UtcNow,
      Items = new List<WebApiCreateSale.CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto Teste",
                    Quantity = 5,
                    UnitPrice = 10.50m
                }
            }
    };

    // Act
    var response = await _httpClient.PostAsJsonAsync("/api/sales", request);

    // Assert
    Assert.True(response.IsSuccessStatusCode, $"Expected success status code, but got {response.StatusCode}");

    var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResult>>();
    Assert.NotNull(result);
    Assert.True(result.Success);
    Assert.NotNull(result.Data);
    Assert.Equal("TEST-001", result.Data!.SaleNumber);

    // Armazenar ID para limpeza
    _createdSaleIds.Add(result.Data.Id);
  }

  /// <summary>
  /// Testa a criação de uma venda com item inválido
  /// </summary>
  [Fact]
  public async Task CreateSale_WithInvalidItem_ShouldReturnBadRequest()
  {
    // Arrange
    var request = new WebApiCreateSale.CreateSaleRequest
    {
      SaleNumber = "TEST-002",
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente Teste 2",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial Teste 2",
      Date = DateTime.UtcNow,
      Items = new List<WebApiCreateSale.CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "", // Nome vazio - inválido
                    Quantity = 3,
                    UnitPrice = 15.00m
                }
            }
    };

    // Act
    var response = await _httpClient.PostAsJsonAsync("/api/sales", request);

    // Assert
    Assert.False(response.IsSuccessStatusCode);
    Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
  }

  /// <summary>
  /// Testa a obtenção de uma venda específica
  /// </summary>
  [Fact]
  public async Task GetSale_WithValidId_ShouldReturnSale()
  {
    // Arrange - Primeiro criar uma venda
    var createRequest = new WebApiCreateSale.CreateSaleRequest
    {
      SaleNumber = "TEST-003",
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente Teste 3",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial Teste 3",
      Date = DateTime.UtcNow,
      Items = new List<WebApiCreateSale.CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto Teste 3",
                    Quantity = 3,
                    UnitPrice = 15.00m
                }
            }
    };

    var createResponse = await _httpClient.PostAsJsonAsync("/api/sales", createRequest);
    Assert.True(createResponse.IsSuccessStatusCode);

    var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResult>>();
    Assert.NotNull(createResult);
    Assert.NotNull(createResult.Data);

    var saleId = createResult.Data!.Id;
    _createdSaleIds.Add(saleId);

    // Act - Buscar a venda criada
    var getResponse = await _httpClient.GetAsync($"/api/sales/{saleId}");

    // Assert
    Assert.True(getResponse.IsSuccessStatusCode);

    var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResponseWithData<GetSaleResult>>();
    Assert.NotNull(getResult);
    Assert.True(getResult.Success);
    Assert.NotNull(getResult.Data);
    Assert.Equal(saleId, getResult.Data!.Id);
    Assert.Equal("TEST-003", getResult.Data.SaleNumber);
  }

  /// <summary>
  /// Testa a obtenção de vendas com filtros
  /// </summary>
  [Fact]
  public async Task GetSales_WithFilters_ShouldReturnFilteredResults()
  {
    // Arrange - Criar algumas vendas para testar filtros
    var customerId = Guid.NewGuid();
    var branchId = Guid.NewGuid();

    var createRequests = new[]
    {
            new WebApiCreateSale.CreateSaleRequest
            {
                SaleNumber = "TEST-FILTER-001",
                CustomerId = customerId,
                CustomerName = "Cliente Filtro",
                BranchId = branchId,
                BranchName = "Filial Filtro",
                Date = DateTime.UtcNow.AddDays(-1),
                Items = new List<WebApiCreateSale.CreateSaleItemRequest>
                {
                    new()
                    {
                        ProductId = Guid.NewGuid(),
                        ProductName = "Produto Filtro 1",
                        Quantity = 2,
                        UnitPrice = 20.00m
                    }
                }
            },
            new WebApiCreateSale.CreateSaleRequest
            {
                SaleNumber = "TEST-FILTER-002",
                CustomerId = customerId,
                CustomerName = "Cliente Filtro",
                BranchId = branchId,
                BranchName = "Filial Filtro",
                Date = DateTime.UtcNow,
                Items = new List<WebApiCreateSale.CreateSaleItemRequest>
                {
                    new()
                    {
                        ProductId = Guid.NewGuid(),
                        ProductName = "Produto Filtro 2",
                        Quantity = 1,
                        UnitPrice = 30.00m
                    }
                }
            }
        };

    foreach (var request in createRequests)
    {
      var response = await _httpClient.PostAsJsonAsync("/api/sales", request);
      Assert.True(response.IsSuccessStatusCode);

      var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResult>>();
      Assert.NotNull(result);
      Assert.NotNull(result.Data);
      _createdSaleIds.Add(result.Data!.Id);
    }

    // Act - Buscar vendas com filtros
    var getResponse = await _httpClient.GetAsync($"/api/sales?customerId={customerId}&branchId={branchId}");

    // Assert
    Assert.True(getResponse.IsSuccessStatusCode);

    var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResponseWithData<GetSalesResult>>();
    Assert.NotNull(getResult);
    Assert.True(getResult.Success);
    Assert.NotNull(getResult.Data);
    Assert.True(getResult.Data!.Sales.Count >= 2);
  }

  /// <summary>
  /// Testa a atualização de uma venda
  /// </summary>
  [Fact]
  public async Task UpdateSale_WithValidData_ShouldReturnSuccess()
  {
    // Arrange - Criar uma venda
    var createRequest = new WebApiCreateSale.CreateSaleRequest
    {
      SaleNumber = "TEST-UPDATE-001",
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente Original",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial Original",
      Date = DateTime.UtcNow,
      Items = new List<WebApiCreateSale.CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto Original",
                    Quantity = 1,
                    UnitPrice = 25.00m
                }
            }
    };

    var createResponse = await _httpClient.PostAsJsonAsync("/api/sales", createRequest);
    Assert.True(createResponse.IsSuccessStatusCode);

    var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResult>>();
    Assert.NotNull(createResult);
    Assert.NotNull(createResult.Data);

    var saleId = createResult.Data!.Id;
    _createdSaleIds.Add(saleId);

    // Act - Atualizar a venda
    var updateRequest = new WebApiUpdateSale.UpdateSaleRequest
    {
      CustomerName = "Cliente Atualizado",
      BranchName = "Filial Atualizada"
    };

    var updateResponse = await _httpClient.PutAsJsonAsync($"/api/sales/{saleId}", updateRequest);

    // Assert
    Assert.True(updateResponse.IsSuccessStatusCode);

    var updateResult = await updateResponse.Content.ReadFromJsonAsync<ApiResponseWithData<UpdateSaleResult>>();
    Assert.NotNull(updateResult);
    Assert.True(updateResult.Success);
    Assert.NotNull(updateResult.Data);
    Assert.Equal(saleId, updateResult.Data!.Id);
  }

  /// <summary>
  /// Testa o cancelamento de uma venda
  /// </summary>
  [Fact]
  public async Task CancelSale_WithValidId_ShouldReturnSuccess()
  {
    // Arrange - Criar uma venda
    var createRequest = new WebApiCreateSale.CreateSaleRequest
    {
      SaleNumber = "TEST-CANCEL-001",
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente Cancelamento",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial Cancelamento",
      Date = DateTime.UtcNow,
      Items = new List<WebApiCreateSale.CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto Cancelamento",
                    Quantity = 1,
                    UnitPrice = 50.00m
                }
            }
    };

    var createResponse = await _httpClient.PostAsJsonAsync("/api/sales", createRequest);
    Assert.True(createResponse.IsSuccessStatusCode);

    var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResult>>();
    Assert.NotNull(createResult);
    Assert.NotNull(createResult.Data);

    var saleId = createResult.Data!.Id;
    _createdSaleIds.Add(saleId);

    // Act - Cancelar a venda
    var cancelResponse = await _httpClient.DeleteAsync($"/api/sales/{saleId}");

    // Assert
    Assert.True(cancelResponse.IsSuccessStatusCode);

    var cancelResult = await cancelResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CancelSaleResult>>();
    Assert.NotNull(cancelResult);
    Assert.True(cancelResult.Success);
    Assert.NotNull(cancelResult.Data);
    Assert.Equal(saleId, cancelResult.Data!.Id);
  }

  /// <summary>
  /// Testa a adição de um item a uma venda
  /// </summary>
  [Fact]
  public async Task AddSaleItem_WithValidData_ShouldReturnSuccess()
  {
    // Arrange - Criar uma venda
    var createRequest = new WebApiCreateSale.CreateSaleRequest
    {
      SaleNumber = "TEST-ADD-ITEM-001",
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente Add Item",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial Add Item",
      Date = DateTime.UtcNow,
      Items = new List<WebApiCreateSale.CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto Original",
                    Quantity = 1,
                    UnitPrice = 25.00m
                }
            }
    };

    var createResponse = await _httpClient.PostAsJsonAsync("/api/sales", createRequest);
    Assert.True(createResponse.IsSuccessStatusCode);

    var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResult>>();
    Assert.NotNull(createResult);
    Assert.NotNull(createResult.Data);

    var saleId = createResult.Data!.Id;
    _createdSaleIds.Add(saleId);

    // Act - Adicionar item à venda
    var addItemRequest = new WebApiAddSaleItem.AddSaleItemRequest
    {
      ProductId = Guid.NewGuid(),
      ProductName = "Produto Adicionado",
      Quantity = 2,
      UnitPrice = 15.00m
    };

    var addItemResponse = await _httpClient.PostAsJsonAsync($"/api/sales/{saleId}/items", addItemRequest);

    // Assert
    Assert.True(addItemResponse.IsSuccessStatusCode);

    var addItemResult = await addItemResponse.Content.ReadFromJsonAsync<ApiResponseWithData<AddSaleItemResult>>();
    Assert.NotNull(addItemResult);
    Assert.True(addItemResult.Success);
    Assert.NotNull(addItemResult.Data);
    Assert.Equal(saleId, addItemResult.Data!.SaleId);
  }

  /// <summary>
  /// Testa a remoção de um item de uma venda
  /// </summary>
  [Fact]
  public async Task RemoveSaleItem_WithValidData_ShouldReturnSuccess()
  {
    // Arrange - Criar uma venda com múltiplos itens
    var createRequest = new WebApiCreateSale.CreateSaleRequest
    {
      SaleNumber = "TEST-REMOVE-ITEM-001",
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente Remove Item",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial Remove Item",
      Date = DateTime.UtcNow,
      Items = new List<WebApiCreateSale.CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto 1",
                    Quantity = 1,
                    UnitPrice = 25.00m
                },
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto 2",
                    Quantity = 2,
                    UnitPrice = 15.00m
                }
            }
    };

    var createResponse = await _httpClient.PostAsJsonAsync("/api/sales", createRequest);
    Assert.True(createResponse.IsSuccessStatusCode);

    var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResult>>();
    Assert.NotNull(createResult);
    Assert.NotNull(createResult.Data);

    var saleId = createResult.Data!.Id;
    _createdSaleIds.Add(saleId);

    // Obter o primeiro item para remoção
    var getResponse = await _httpClient.GetAsync($"/api/sales/{saleId}");
    Assert.True(getResponse.IsSuccessStatusCode);

    var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResponseWithData<GetSaleResult>>();
    Assert.NotNull(getResult);
    Assert.NotNull(getResult.Data);
    Assert.True(getResult.Data!.Items.Count >= 2);

    var itemToRemove = getResult.Data.Items.First();
    var itemId = itemToRemove.Id;

    // Act - Remover o item
    var removeItemResponse = await _httpClient.DeleteAsync($"/api/sales/{saleId}/items/{itemId}");

    // Assert
    Assert.True(removeItemResponse.IsSuccessStatusCode);

    var removeItemResult = await removeItemResponse.Content.ReadFromJsonAsync<ApiResponseWithData<RemoveSaleItemResult>>();
    Assert.NotNull(removeItemResult);
    Assert.True(removeItemResult.Success);
    Assert.NotNull(removeItemResult.Data);
    Assert.Equal(saleId, removeItemResult.Data!.SaleId);
    Assert.Equal(itemId, removeItemResult.Data.RemovedItemId);
  }
}
