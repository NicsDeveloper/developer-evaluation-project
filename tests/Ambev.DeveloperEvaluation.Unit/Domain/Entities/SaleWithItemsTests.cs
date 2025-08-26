using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleWithItemsTests
{
  [Fact]
  public void AddItem_ShouldAddItemToSaleAndRecalculateTotals()
  {
    // Arrange
    var sale = Sale.Create("SALE001", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    var item = SaleItem.Create(Guid.NewGuid(), "Produto Teste", 2, 10.00m, sale.Id);

    // Act
    sale.AddItem(item);

    // Assert
    Assert.Single(sale.Items);
    Assert.Contains(item, sale.Items);
    Assert.Equal(20.00m, sale.GrossTotal);
    Assert.Equal(0.00m, sale.DiscountTotal); // 0% para 2 itens
    Assert.Equal(20.00m, sale.NetTotal);
  }

  [Fact]
  public void AddMultipleItems_ShouldCalculateTotalsCorrectly()
  {
    // Arrange
    var sale = Sale.Create("SALE002", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    var item1 = SaleItem.Create(Guid.NewGuid(), "Produto 1", 5, 10.00m, sale.Id);
    var item2 = SaleItem.Create(Guid.NewGuid(), "Produto 2", 10, 20.00m, sale.Id);

    // Act
    sale.AddItem(item1);
    sale.AddItem(item2);

    // Assert
    Assert.Equal(2, sale.Items.Count);
    Assert.Equal(250.00m, sale.GrossTotal); // (5 * 10) + (10 * 20)
    Assert.Equal(22.50m, sale.DiscountTotal); // (5 * 10 * 0.05) + (10 * 20 * 0.10)
    Assert.Equal(227.50m, sale.NetTotal); // 250 - 22.50
  }

  [Fact]
  public void RemoveItem_ShouldRemoveItemAndRecalculateTotals()
  {
    // Arrange
    var sale = Sale.Create("SALE003", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    var item1 = SaleItem.Create(Guid.NewGuid(), "Produto 1", 5, 10.00m, sale.Id);
    var item2 = SaleItem.Create(Guid.NewGuid(), "Produto 2", 10, 20.00m, sale.Id);
    sale.AddItem(item1);
    sale.AddItem(item2);

    // Act
    sale.RemoveItem(item1.Id);

    // Assert
    Assert.Single(sale.Items);
    Assert.Contains(item2, sale.Items);
    Assert.False(sale.Items.Any(i => i.Id == item1.Id), $"Item1 with ID {item1.Id} should not be in the collection");
    Assert.Equal(200.00m, sale.GrossTotal); // 10 * 20
    Assert.Equal(20.00m, sale.DiscountTotal); // 10 * 20 * 0.10
    Assert.Equal(180.00m, sale.NetTotal); // 200 - 20
  }

  [Fact]
  public void RemoveNonExistentItem_ShouldNotChangeSale()
  {
    // Arrange
    var sale = Sale.Create("SALE004", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    var item = SaleItem.Create(Guid.NewGuid(), "Produto Teste", 5, 10.00m, sale.Id);
    sale.AddItem(item);
    var originalGrossTotal = sale.GrossTotal;

    // Act
    sale.RemoveItem(Guid.NewGuid()); // ID que não existe

    // Assert
    Assert.Single(sale.Items);
    Assert.Equal(originalGrossTotal, sale.GrossTotal);
  }

  [Fact]
  public void RecalculateTotals_ShouldUpdateAllTotalsCorrectly()
  {
    // Arrange
    var sale = Sale.Create("SALE005", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    var item1 = SaleItem.Create(Guid.NewGuid(), "Produto 1", 15, 10.00m, sale.Id);
    var item2 = SaleItem.Create(Guid.NewGuid(), "Produto 2", 20, 5.00m, sale.Id);
    sale.AddItem(item1);
    sale.AddItem(item2);

    // Act
    sale.RecalculateTotals();

    // Assert
    Assert.Equal(250.00m, sale.GrossTotal); // (15 * 10) + (20 * 5)
    Assert.Equal(42.50m, sale.DiscountTotal); // (15 * 10 * 0.15) + (20 * 5 * 0.20)
    Assert.Equal(207.50m, sale.NetTotal); // 250 - 42.50
  }

  [Fact]
  public void CancelSale_ShouldCancelSaleAndAllItems()
  {
    // Arrange
    var sale = Sale.Create("SALE006", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    var item1 = SaleItem.Create(Guid.NewGuid(), "Produto 1", 5, 10.00m, sale.Id);
    var item2 = SaleItem.Create(Guid.NewGuid(), "Produto 2", 10, 20.00m, sale.Id);
    sale.AddItem(item1);
    sale.AddItem(item2);

    // Act
    sale.Cancel();

    // Assert
    Assert.True(sale.Cancelled);
    Assert.True(item1.Cancelled);
    Assert.True(item2.Cancelled);
  }

  [Fact]
  public void UpdateSale_ShouldUpdatePropertiesCorrectly()
  {
    // Arrange
    var sale = Sale.Create("SALE007", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    var newCustomerName = "Novo Cliente";
    var newBranchName = "Nova Filial";

    // Act
    sale.Update(newCustomerName, newBranchName);

    // Assert
    Assert.Equal(newCustomerName, sale.CustomerName);
    Assert.Equal(newBranchName, sale.BranchName);
    Assert.NotEqual(DateTime.MinValue, sale.UpdatedAt);
  }

  [Fact]
  public void SaleWithMaximumItems_ShouldCalculateCorrectDiscount()
  {
    // Arrange
    var sale = Sale.Create("SALE008", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");
    var item = SaleItem.Create(Guid.NewGuid(), "Produto Teste", 20, 10.00m, sale.Id);

    // Act
    sale.AddItem(item);

    // Assert
    Assert.Equal(200.00m, sale.GrossTotal); // 20 * 10
    Assert.Equal(40.00m, sale.DiscountTotal); // 20 * 10 * 0.20 (máximo desconto)
    Assert.Equal(160.00m, sale.NetTotal); // 200 - 40
  }

  [Fact]
  public void SaleWithZeroItems_ShouldHaveZeroTotals()
  {
    // Arrange
    var sale = Sale.Create("SALE009", Guid.NewGuid(), "Cliente Teste", Guid.NewGuid(), "Filial Teste");

    // Act & Assert
    Assert.Empty(sale.Items);
    Assert.Equal(0.00m, sale.GrossTotal);
    Assert.Equal(0.00m, sale.DiscountTotal);
    Assert.Equal(0.00m, sale.NetTotal);
  }
}
