using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Services;

/// <summary>
/// Provides business rules for calculating discounts based on quantity.
/// </summary>
public static class DiscountRules
{
  /// <summary>
  /// Calculates the discount percentage and amount based on quantity and unit price.
  /// </summary>
  /// <param name="quantity">The quantity of items</param>
  /// <param name="unitPrice">The unit price of the item</param>
  /// <returns>A tuple containing discount percentage and amount</returns>
  /// <exception cref="DomainException">Thrown when quantity is invalid</exception>
  public static (decimal DiscountPercent, decimal DiscountAmount) CalculateDiscount(int quantity, decimal unitPrice)
  {
    if (quantity <= 0)
      throw new DomainException("Quantidade deve ser maior que zero");

    if (quantity > 20)
      throw new DomainException("Quantidade máxima permitida é 20 itens");

    var discountPercent = CalculateDiscountPercent(quantity);
    var discountAmount = quantity * unitPrice * discountPercent;

    return (discountPercent, discountAmount);
  }

  /// <summary>
  /// Calculates the discount percentage based on quantity according to business rules.
  /// </summary>
  /// <param name="quantity">The quantity of items</param>
  /// <returns>The discount percentage as a decimal</returns>
  public static decimal CalculateDiscountPercent(int quantity)
  {
    return quantity switch
    {
      >= 10 => 0.20m,  // 20% discount for 10-20 items
      >= 4 => 0.10m,   // 10% discount for 4-9 items
      _ => 0.00m        // No discount for 1-3 items
    };
  }

  /// <summary>
  /// Validates if a quantity is valid according to business rules.
  /// </summary>
  /// <param name="quantity">The quantity to validate</param>
  /// <returns>True if quantity is valid, false otherwise</returns>
  public static bool IsValidQuantity(int quantity)
  {
    return quantity > 0 && quantity <= 20;
  }

  /// <summary>
  /// Gets the maximum allowed quantity per item.
  /// </summary>
  /// <returns>The maximum quantity allowed</returns>
  public static int GetMaxQuantity() => 20;

  /// <summary>
  /// Gets the minimum quantity required for any discount.
  /// </summary>
  /// <returns>The minimum quantity for discount</returns>
  public static int GetMinQuantityForDiscount() => 4;

  /// <summary>
  /// Gets the minimum quantity required for maximum discount (20%).
  /// </summary>
  /// <returns>The minimum quantity for maximum discount</returns>
  public static int GetMinQuantityForMaxDiscount() => 10;
}
