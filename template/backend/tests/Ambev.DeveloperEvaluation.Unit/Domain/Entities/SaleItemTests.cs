using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the SaleItem entity class.
/// Tests cover basic properties and creation scenarios.
/// </summary>
public class SaleItemTests
{
  [Fact(DisplayName = "SaleItem should be created with correct basic properties")]
  public void Given_ValidParameters_When_CreateSaleItem_Then_ShouldHaveCorrectProperties()
  {
    // Arrange
    var productId = Guid.NewGuid();
    var productName = "Produto Teste";
    var quantity = 5;
    var unitPrice = 10.50m;
    var saleId = Guid.NewGuid();

    // Act
    var item = SaleItem.Create(productId, productName, quantity, unitPrice, saleId);

    // Assert
    Assert.Equal(productId, item.ProductId);
    Assert.Equal(productName, item.ProductName);
    Assert.Equal(quantity, item.Quantity);
    Assert.Equal(unitPrice, item.UnitPrice);
    Assert.Equal(saleId, item.SaleId);
    Assert.False(item.Cancelled);
    Assert.NotEqual(DateTime.MinValue, item.CreatedAt);
    Assert.Equal(52.50m, item.GrossAmount); // 5 * 10.50
    Assert.Equal(5.25m, item.DiscountAmount); // 10% discount for quantity 5
    Assert.Equal(47.25m, item.NetAmount); // 52.50 - 5.25
  }

  [Fact(DisplayName = "SaleItem should have correct initial state")]
  public void Given_NewSaleItem_When_Created_Then_ShouldHaveCorrectInitialState()
  {
    // Act
    var item = new SaleItem();

    // Assert
    Assert.Equal(Guid.Empty, item.ProductId);
    Assert.Equal(string.Empty, item.ProductName);
    Assert.Equal(0, item.Quantity);
    Assert.Equal(0, item.UnitPrice);
    Assert.Equal(0, item.DiscountPercent);
    Assert.Equal(0, item.DiscountAmount);
    Assert.Equal(0, item.GrossAmount);
    Assert.Equal(0, item.NetAmount);
    Assert.False(item.Cancelled);
    Assert.Equal(Guid.Empty, item.SaleId);
    Assert.NotEqual(DateTime.MinValue, item.CreatedAt);
  }

  [Fact(DisplayName = "SaleItem should throw exception when created with empty product name")]
  public void Given_EmptyProductName_When_CreateSaleItem_Then_ShouldThrowArgumentException()
  {
    // Arrange
    var productId = Guid.NewGuid();
    var quantity = 5;
    var unitPrice = 10.50m;
    var saleId = Guid.NewGuid();

    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(() =>
        SaleItem.Create(productId, "", quantity, unitPrice, saleId));
    Assert.Contains("Product name cannot be null or empty", exception.Message);
  }

  [Fact(DisplayName = "SaleItem should throw exception when created with zero quantity")]
  public void Given_ZeroQuantity_When_CreateSaleItem_Then_ShouldThrowArgumentException()
  {
    // Arrange
    var productId = Guid.NewGuid();
    var productName = "Produto Teste";
    var unitPrice = 10.50m;
    var saleId = Guid.NewGuid();

    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(() =>
        SaleItem.Create(productId, productName, 0, unitPrice, saleId));
    Assert.Contains("Quantity must be greater than zero", exception.Message);
  }

  [Fact(DisplayName = "SaleItem should throw exception when created with negative quantity")]
  public void Given_NegativeQuantity_When_CreateSaleItem_Then_ShouldThrowArgumentException()
  {
    // Arrange
    var productId = Guid.NewGuid();
    var productName = "Produto Teste";
    var unitPrice = 10.50m;
    var saleId = Guid.NewGuid();

    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(() =>
        SaleItem.Create(productId, productName, -1, unitPrice, saleId));
    Assert.Contains("Quantity must be greater than zero", exception.Message);
  }

  [Fact(DisplayName = "SaleItem should throw exception when created with quantity above 20")]
  public void Given_QuantityAbove20_When_CreateSaleItem_Then_ShouldThrowArgumentException()
  {
    // Arrange
    var productId = Guid.NewGuid();
    var productName = "Produto Teste";
    var unitPrice = 10.50m;
    var saleId = Guid.NewGuid();

    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(() =>
        SaleItem.Create(productId, productName, 21, unitPrice, saleId));
    Assert.Contains("Quantity cannot exceed 20 items", exception.Message);
  }

  [Fact(DisplayName = "SaleItem should throw exception when created with negative unit price")]
  public void Given_NegativeUnitPrice_When_CreateSaleItem_Then_ShouldThrowArgumentException()
  {
    // Arrange
    var productId = Guid.NewGuid();
    var productName = "Produto Teste";
    var quantity = 5;
    var saleId = Guid.NewGuid();

    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(() =>
        SaleItem.Create(productId, productName, quantity, -10.50m, saleId));
    Assert.Contains("Unit price cannot be negative", exception.Message);
  }
}
