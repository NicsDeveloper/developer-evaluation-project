using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
  /// <summary>
  /// Represents a sale transaction in the system.
  /// This entity follows domain-driven design principles and includes business rules validation.
  /// </summary>
  public class Sale : BaseEntity
  {
    /// <summary>
    /// Gets the unique sale number for this transaction.
    /// Format: SAL-yyyymmdd-#### or BRANCH-yyyymmdd-####
    /// </summary>
    public string SaleNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the date when the sale was made.
    /// </summary>
    public DateTime Date { get; private set; }

    /// <summary>
    /// Gets the external customer identifier.
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Gets the denormalized customer name for quick access.
    /// </summary>
    public string CustomerName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the external branch identifier where the sale was made.
    /// </summary>
    public Guid BranchId { get; private set; }

    /// <summary>
    /// Gets the denormalized branch name for quick access.
    /// </summary>
    public string BranchName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the gross total amount before discounts.
    /// </summary>
    public decimal GrossTotal { get; private set; }

    /// <summary>
    /// Gets the total discount amount applied.
    /// </summary>
    public decimal DiscountTotal { get; private set; }

    /// <summary>
    /// Gets the net total amount after discounts.
    /// </summary>
    public decimal NetTotal { get; private set; }

    /// <summary>
    /// Gets whether the sale has been cancelled.
    /// </summary>
    public bool Cancelled { get; private set; }

    /// <summary>
    /// Gets the collection of sale items.
    /// </summary>
    public ICollection<SaleItem> Items { get; private set; } = [];

    /// <summary>
    /// Gets the date and time when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Sale class.
    /// </summary>
    public Sale()
    {
      Date = DateTime.UtcNow;
      CreatedAt = DateTime.UtcNow;
      GrossTotal = 0;
      DiscountTotal = 0;
      NetTotal = 0;
      Cancelled = false;
    }

    /// <summary>
    /// Creates a new sale with the specified details.
    /// </summary>
    /// <param name="saleNumber">The unique sale number</param>
    /// <param name="customerId">The customer identifier</param>
    /// <param name="customerName">The customer name</param>
    /// <param name="branchId">The branch identifier</param>
    /// <param name="branchName">The branch name</param>
    /// <returns>A new Sale instance</returns>
    public static Sale Create(string saleNumber, Guid customerId, string customerName, Guid branchId, string branchName)
    {
      return string.IsNullOrWhiteSpace(saleNumber)
        ? throw new ArgumentException("Sale number cannot be null or empty", nameof(saleNumber))
        : string.IsNullOrWhiteSpace(customerName)
        ? throw new ArgumentException("Customer name cannot be null or empty", nameof(customerName))
        : string.IsNullOrWhiteSpace(branchName)
            ? throw new ArgumentException("Branch name cannot be null or empty", nameof(branchName))
            : new Sale
            {
              SaleNumber = saleNumber,
              CustomerId = customerId,
              CustomerName = customerName,
              BranchId = branchId,
              BranchName = branchName
            };
    }

    /// <summary>
    /// Adds an item to the sale.
    /// </summary>
    /// <param name="item">The sale item to add</param>
    public void AddItem(SaleItem item)
    {
      ArgumentNullException.ThrowIfNull(item);

      if (Cancelled)
      {
        throw new InvalidOperationException("Cannot add items to a cancelled sale");
      }

      Items.Add(item);
      RecalculateTotals();
      UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes an item from the sale.
    /// </summary>
    /// <param name="itemId">The ID of the item to remove</param>
    public void RemoveItem(Guid itemId)
    {
      if (Cancelled)
      {
        throw new InvalidOperationException("Cannot remove items from a cancelled sale");
      }

      SaleItem? item = Items.FirstOrDefault(i => i.Id == itemId);
      if (item != null)
      {
        _ = Items.Remove(item);
        RecalculateTotals();
        UpdatedAt = DateTime.UtcNow;
      }
    }

    /// <summary>
    /// Recalculates all totals based on the current items.
    /// </summary>
    public void RecalculateTotals()
    {
      IEnumerable<SaleItem> activeItems = Items.Where(static i => !i.Cancelled);

      GrossTotal = activeItems.Sum(static i => i.GrossAmount);
      DiscountTotal = activeItems.Sum(static i => i.DiscountAmount);
      NetTotal = activeItems.Sum(static i => i.NetAmount);
    }

    /// <summary>
    /// Cancels the entire sale.
    /// </summary>
    public void Cancel()
    {
      if (Cancelled)
      {
        throw new InvalidOperationException("Sale is already cancelled");
      }

      Cancelled = true;

      // Cancel all items
      foreach (SaleItem item in Items)
      {
        item.Cancel();
      }

      UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the sale information.
    /// </summary>
    /// <param name="customerName">The new customer name</param>
    /// <param name="branchName">The new branch name</param>
    public void Update(string customerName, string branchName)
    {
      if (Cancelled)
      {
        throw new InvalidOperationException("Cannot update a cancelled sale");
      }

      if (!string.IsNullOrWhiteSpace(customerName))
      {
        CustomerName = customerName;
      }

      if (!string.IsNullOrWhiteSpace(branchName))
      {
        BranchName = branchName;
      }

      UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the sale date (for testing purposes).
    /// </summary>
    /// <param name="date">The date to set</param>
    public void SetDate(DateTime date)
    {
      if (Cancelled)
      {
        throw new InvalidOperationException("Cannot update a cancelled sale");
      }

      Date = date;
      UpdatedAt = DateTime.UtcNow;
    }
  }
}
