using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Services;

public class DiscountRulesTests
{
    [Fact]
    public void CalculateDiscount_WithValidQuantity_ShouldReturnCorrectDiscount()
    {
        // Arrange
        var quantity = 5;
        var unitPrice = 10.00m;

        // Act
        var (discountPercent, discountAmount) = DiscountRules.CalculateDiscount(quantity, unitPrice);

        // Assert
        Assert.Equal(0.05m, discountPercent); // 5% para 5 itens
        Assert.Equal(2.50m, discountAmount); // 5 * 10 * 0.05
    }

    [Fact]
    public void CalculateDiscount_WithQuantityOne_ShouldReturnNoDiscount()
    {
        // Arrange
        var quantity = 1;
        var unitPrice = 10.00m;

        // Act
        var (discountPercent, discountAmount) = DiscountRules.CalculateDiscount(quantity, unitPrice);

        // Assert
        Assert.Equal(0.00m, discountPercent); // 0% para 1 item
        Assert.Equal(0.00m, discountAmount); // 1 * 10 * 0.00
    }

    [Fact]
    public void CalculateDiscount_WithQuantityTen_ShouldReturnCorrectDiscount()
    {
        // Arrange
        var quantity = 10;
        var unitPrice = 10.00m;

        // Act
        var (discountPercent, discountAmount) = DiscountRules.CalculateDiscount(quantity, unitPrice);

        // Assert
        Assert.Equal(0.10m, discountPercent); // 10% para 10 itens
        Assert.Equal(10.00m, discountAmount); // 10 * 10 * 0.10
    }

    [Fact]
    public void CalculateDiscount_WithQuantityTwenty_ShouldReturnMaximumDiscount()
    {
        // Arrange
        var quantity = 20;
        var unitPrice = 10.00m;

        // Act
        var (discountPercent, discountAmount) = DiscountRules.CalculateDiscount(quantity, unitPrice);

        // Assert
        Assert.Equal(0.20m, discountPercent); // 20% para 20 itens
        Assert.Equal(40.00m, discountAmount); // 20 * 10 * 0.20
    }

    [Fact]
    public void CalculateDiscount_WithZeroQuantity_ShouldThrowException()
    {
        // Arrange
        var quantity = 0;
        var unitPrice = 10.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            DiscountRules.CalculateDiscount(quantity, unitPrice));
        Assert.Contains("Quantidade deve ser maior que zero", exception.Message);
    }

    [Fact]
    public void CalculateDiscount_WithNegativeQuantity_ShouldThrowException()
    {
        // Arrange
        var quantity = -1;
        var unitPrice = 10.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            DiscountRules.CalculateDiscount(quantity, unitPrice));
        Assert.Contains("Quantidade deve ser maior que zero", exception.Message);
    }

    [Fact]
    public void CalculateDiscount_WithQuantityOverTwenty_ShouldThrowException()
    {
        // Arrange
        var quantity = 21;
        var unitPrice = 10.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            DiscountRules.CalculateDiscount(quantity, unitPrice));
        Assert.Contains("Quantidade máxima permitida é 20 itens", exception.Message);
    }

    [Fact]
    public void CalculateDiscount_WithHighUnitPrice_ShouldReturnCorrectDiscount()
    {
        // Arrange
        var quantity = 15;
        var unitPrice = 100.00m;

        // Act
        var (discountPercent, discountAmount) = DiscountRules.CalculateDiscount(quantity, unitPrice);

        // Assert
        Assert.Equal(0.15m, discountPercent); // 15% para 15 itens
        Assert.Equal(225.00m, discountAmount); // 15 * 100 * 0.15
    }

    [Fact]
    public void CalculateDiscountPercent_WithDifferentQuantities_ShouldReturnCorrectPercentages()
    {
        // Arrange & Act & Assert
        Assert.Equal(0.00m, DiscountRules.CalculateDiscountPercent(1));
        Assert.Equal(0.05m, DiscountRules.CalculateDiscountPercent(5));
        Assert.Equal(0.10m, DiscountRules.CalculateDiscountPercent(10));
        Assert.Equal(0.15m, DiscountRules.CalculateDiscountPercent(15));
        Assert.Equal(0.20m, DiscountRules.CalculateDiscountPercent(20));
    }

    [Fact]
    public void IsValidQuantity_WithValidQuantities_ShouldReturnTrue()
    {
        // Arrange & Act & Assert
        Assert.True(DiscountRules.IsValidQuantity(1));
        Assert.True(DiscountRules.IsValidQuantity(10));
        Assert.True(DiscountRules.IsValidQuantity(20));
    }

    [Fact]
    public void IsValidQuantity_WithInvalidQuantities_ShouldReturnFalse()
    {
        // Arrange & Act & Assert
        Assert.False(DiscountRules.IsValidQuantity(0));
        Assert.False(DiscountRules.IsValidQuantity(-1));
        Assert.False(DiscountRules.IsValidQuantity(21));
    }
}
