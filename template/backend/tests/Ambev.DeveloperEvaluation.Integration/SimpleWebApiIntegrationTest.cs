using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

public class SimpleWebApiIntegrationTest : IDisposable
{
    private readonly TestServer _testServer;
    private readonly HttpClient _httpClient;

    public SimpleWebApiIntegrationTest()
    {
        var builder = new WebHostBuilder()
            .UseStartup<TestStartup>();

        _testServer = new TestServer(builder);
        _httpClient = _testServer.CreateClient();
    }

    [Fact]
    public async Task CreateSale_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = new WebApi.Features.Sales.CreateSale.CreateSaleRequest
        {
            SaleNumber = "TEST-001",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente Teste",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Teste",
            Items = new List<WebApi.Features.Sales.CreateSale.CreateSaleItemRequest>
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
        var content = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, 
            $"Expected success status code, but got {response.StatusCode}. Content: {content}");
        
        // Log the response content to understand the structure
        Console.WriteLine($"Response Content: {content}");
        
        Assert.Contains("TEST-001", content);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        _testServer?.Dispose();
    }
}
