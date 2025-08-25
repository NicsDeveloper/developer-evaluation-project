using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(i => i.ProductId)
            .IsRequired();

        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.DiscountPercent)
            .HasColumnType("decimal(5,4)")
            .HasPrecision(5, 4)
            .IsRequired();

        builder.Property(i => i.DiscountAmount)
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.GrossAmount)
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.NetAmount)
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.Cancelled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(i => i.SaleId)
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.UpdatedAt);

        // Índices para consultas frequentes
        builder.HasIndex(i => i.ProductId);
        builder.HasIndex(i => i.SaleId);
        builder.HasIndex(i => i.Cancelled);

        // Relacionamento com Sale
        builder.HasOne(i => i.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
