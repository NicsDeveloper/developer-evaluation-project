using Ambev.DeveloperEvaluation.Common.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Common
{
  public class BaseEntity : IComparable<BaseEntity>
  {
    public Guid Id { get; set; } = Guid.NewGuid();

    public Task<IEnumerable<ValidationErrorDetail>> ValidateAsync()
    {
      return Validator.ValidateAsync(this);
    }

    public int CompareTo(BaseEntity? other)
    {
      return other == null ? 1 : other!.Id.CompareTo(Id);
    }

    public override bool Equals(object? obj)
    {
      return ReferenceEquals(this, obj);
    }

    public override int GetHashCode()
    {
      return Id.GetHashCode();
    }

    public static bool operator ==(BaseEntity? left, BaseEntity? right)
    {
      return ReferenceEquals(left, right);
    }

    public static bool operator !=(BaseEntity? left, BaseEntity? right)
    {
      return !(left == right);
    }

    public static bool operator <(BaseEntity? left, BaseEntity? right)
    {
      return left is null ? right is not null : left.CompareTo(right) < 0;
    }

    public static bool operator <=(BaseEntity? left, BaseEntity? right)
    {
      return left is null || left.CompareTo(right) <= 0;
    }

    public static bool operator >(BaseEntity? left, BaseEntity? right)
    {
      return left is not null && left.CompareTo(right) > 0;
    }

    public static bool operator >=(BaseEntity? left, BaseEntity? right)
    {
      return left is null ? right is null : left.CompareTo(right) >= 0;
    }
  }
}