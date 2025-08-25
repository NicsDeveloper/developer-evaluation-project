using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover basic properties and creation scenarios.
/// </summary>
public class SaleTests
{
  [Fact(DisplayName = "Sale should be created with correct basic properties")]
  public void Given_ValidParameters_When_CreateSale_Then_ShouldHaveCorrectProperties()
  {
    // Arrange
    var saleNumber = "SAL-20241201-0001";
    var customerId = Guid.NewGuid();
    var customerName = "João Silva";
    var branchId = Guid.NewGuid();
    var branchName = "Filial Centro";

    // Act
    var sale = Sale.Create(saleNumber, customerId, customerName, branchId, branchName);

    // Assert
    Assert.Equal(saleNumber, sale.SaleNumber);
    Assert.Equal(customerId, sale.CustomerId);
    Assert.Equal(customerName, sale.CustomerName);
    Assert.Equal(branchId, sale.BranchId);
    Assert.Equal(branchName, sale.BranchName);
    Assert.False(sale.Cancelled);
    Assert.Equal(0, sale.GrossTotal);
    Assert.Equal(0, sale.DiscountTotal);
    Assert.Equal(0, sale.NetTotal);
    Assert.Empty(sale.Items);
    Assert.NotEqual(DateTime.MinValue, sale.CreatedAt);
    Assert.NotEqual(DateTime.MinValue, sale.Date);
  }

  [Fact(DisplayName = "Sale should have correct initial state")]
  public void Given_NewSale_When_Created_Then_ShouldHaveCorrectInitialState()
  {
    // Act
    var sale = new Sale();

    // Assert
    Assert.False(sale.Cancelled);
    Assert.Equal(0, sale.GrossTotal);
    Assert.Equal(0, sale.DiscountTotal);
    Assert.Equal(0, sale.NetTotal);
    Assert.Empty(sale.Items);
    Assert.NotEqual(DateTime.MinValue, sale.CreatedAt);
    Assert.NotEqual(DateTime.MinValue, sale.Date);
  }

  [Fact(DisplayName = "Sale should throw exception when created with empty sale number")]
  public void Given_EmptySaleNumber_When_CreateSale_Then_ShouldThrowArgumentException()
  {
    // Arrange
    var customerId = Guid.NewGuid();
    var customerName = "João Silva";
    var branchId = Guid.NewGuid();
    var branchName = "Filial Centro";

    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(() =>
        Sale.Create("", customerId, customerName, branchId, branchName));
    Assert.Contains("Sale number cannot be null or empty", exception.Message);
  }

  [Fact(DisplayName = "Sale should throw exception when created with empty customer name")]
  public void Given_EmptyCustomerName_When_CreateSale_Then_ShouldThrowArgumentException()
  {
    // Arrange
    var saleNumber = "SAL-20241201-0001";
    var customerId = Guid.NewGuid();
    var branchId = Guid.NewGuid();
    var branchName = "Filial Centro";

    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(() =>
        Sale.Create(saleNumber, customerId, "", branchId, branchName));
    Assert.Contains("Customer name cannot be null or empty", exception.Message);
  }

  [Fact(DisplayName = "Sale should throw exception when created with empty branch name")]
  public void Given_EmptyBranchName_When_CreateSale_Then_ShouldThrowArgumentException()
  {
    // Arrange
    var saleNumber = "SAL-20241201-0001";
    var customerId = Guid.NewGuid();
    var customerName = "João Silva";
    var branchId = Guid.NewGuid();

    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(() =>
        Sale.Create(saleNumber, customerId, customerName, branchId, ""));
    Assert.Contains("Branch name cannot be null or empty", exception.Message);
  }
}
