using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Services;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
  /// <summary>
  /// Represents an individual item within a sale transaction.
  /// This entity follows domain-driven design principles and includes business rules validation.
  /// </summary>
  public class SaleItem : BaseEntity
  {
    /// <summary>
    /// Gets the external product identifier.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Gets the denormalized product name for quick access.
    /// </summary>
    public string ProductName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the quantity of the product sold.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Gets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Gets the discount percentage applied to this item.
    /// </summary>
    public decimal DiscountPercent { get; private set; }

    /// <summary>
    /// Gets the discount amount applied to this item.
    /// </summary>
    public decimal DiscountAmount { get; private set; }

    /// <summary>
    /// Gets the gross amount before discount (quantity * unit price).
    /// </summary>
    public decimal GrossAmount { get; private set; }

    /// <summary>
    /// Gets the net amount after discount.
    /// </summary>
    public decimal NetAmount { get; private set; }

    /// <summary>
    /// Gets whether this item has been cancelled.
    /// </summary>
    public bool Cancelled { get; private set; }

    /// <summary>
    /// Gets the sale that this item belongs to.
    /// </summary>
    public Guid SaleId { get; private set; }

    /// <summary>
    /// Gets the sale navigation property.
    /// </summary>
    public Sale Sale { get; private set; } = null!;

    /// <summary>
    /// Gets the date and time when the item was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time of the last update to the item.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the SaleItem class.
    /// </summary>
    public SaleItem()
    {
      CreatedAt = DateTime.UtcNow;
      Quantity = 0;
      UnitPrice = 0;
      DiscountPercent = 0;
      DiscountAmount = 0;
      GrossAmount = 0;
      NetAmount = 0;
      Cancelled = false;
    }

    /// <summary>
    /// Creates a new sale item with the specified details.
    /// </summary>
    /// <param name="productId">The product identifier</param>
    /// <param name="productName">The product name</param>
    /// <param name="quantity">The quantity to sell</param>
    /// <param name="unitPrice">The unit price</param>
    /// <param name="saleId">The sale identifier</param>
    /// <returns>A new SaleItem instance</returns>
    public static SaleItem Create(Guid productId, string productName, int quantity, decimal unitPrice, Guid saleId)
    {
      if (string.IsNullOrWhiteSpace(productName))
      {
        throw new ArgumentException("Product name cannot be null or empty", nameof(productName));
      }

      if (quantity <= 0)
      {
        throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
      }

      if (quantity > 20)
      {
        throw new ArgumentException("Quantity cannot exceed 20 items", nameof(quantity));
      }

      if (unitPrice < 0)
      {
        throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));
      }

      SaleItem item = new()
      {
        ProductId = productId,
        ProductName = productName,
        Quantity = quantity,
        UnitPrice = unitPrice,
        SaleId = saleId
      };

      item.CalculateAmounts();
      return item;
    }

    /// <summary>
    /// Calculates all amounts for this item based on quantity, unit price, and discount rules.
    /// </summary>
    public void CalculateAmounts()
    {
      // Calculate gross amount
      GrossAmount = Quantity * UnitPrice;

      // Apply discount rules based on quantity using the DiscountRules service
      (decimal discountPercent, decimal discountAmount) = DiscountRules.CalculateDiscount(Quantity, UnitPrice);
      DiscountPercent = discountPercent;
      DiscountAmount = discountAmount;

      // Calculate net amount
      NetAmount = GrossAmount - DiscountAmount;

      UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the item details and recalculates amounts.
    /// </summary>
    /// <param name="productName">The new product name</param>
    /// <param name="quantity">The new quantity</param>
    /// <param name="unitPrice">The new unit price</param>
    public void Update(string productName, int quantity, decimal unitPrice)
    {
      if (Cancelled)
      {
        throw new InvalidOperationException("Cannot update a cancelled item");
      }

      if (string.IsNullOrWhiteSpace(productName))
      {
        throw new ArgumentException("Product name cannot be null or empty", nameof(productName));
      }

      if (quantity <= 0)
      {
        throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
      }

      if (quantity > 20)
      {
        throw new ArgumentException("Quantity cannot exceed 20 items", nameof(quantity));
      }

      if (unitPrice < 0)
      {
        throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));
      }

      ProductName = productName;
      Quantity = quantity;
      UnitPrice = unitPrice;

      CalculateAmounts();
    }

    /// <summary>
    /// Cancels this specific item.
    /// </summary>
    public void Cancel()
    {
      if (Cancelled)
      {
        throw new InvalidOperationException("Item is already cancelled");
      }

      Cancelled = true;
      UpdatedAt = DateTime.UtcNow;
    }
  }
}
