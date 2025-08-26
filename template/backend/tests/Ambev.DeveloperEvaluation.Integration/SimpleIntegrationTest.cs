using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

public class SimpleIntegrationTest
{
  [Fact]
  public async Task CreateSale_WithValidData_ShouldWork()
  {
    // Arrange
    var services = new ServiceCollection();
    services.AddDbContext<DefaultContext>(options =>
        options.UseInMemoryDatabase("SimpleTest_" + Guid.NewGuid())
    );
    services.AddScoped<Ambev.DeveloperEvaluation.Domain.Repositories.ISaleRepository, Ambev.DeveloperEvaluation.ORM.Repositories.SaleRepository>();
    services.AddMediatR(cfg =>
    {
      cfg.RegisterServicesFromAssembly(typeof(CreateSaleHandler).Assembly);
    });

    var serviceProvider = services.BuildServiceProvider();
    var context = serviceProvider.GetRequiredService<DefaultContext>();
    var mediator = serviceProvider.GetRequiredService<IMediator>();

    // Act
    var command = new CreateSaleCommand
    {
      SaleNumber = "SIMPLE-001",
      CustomerId = Guid.NewGuid(),
      CustomerName = "Cliente Teste",
      BranchId = Guid.NewGuid(),
      BranchName = "Filial Teste",
      Items = new List<CreateSaleItemRequest>
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

    var result = await mediator.Send(command);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("SIMPLE-001", result.SaleNumber);
    Assert.Single(result.Items);
    Assert.Equal(52.50m, result.GrossTotal); // 5 * 10.50
    Assert.Equal(2.625m, result.DiscountTotal); // 5% discount for quantity 5 (52.50 * 0.05)
    Assert.Equal(49.875m, result.NetTotal); // 52.50 - 2.625
  }
}
