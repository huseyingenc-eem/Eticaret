using ETicaret.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Persistence.Configurations;

public class OperationClaimConfiguration : IEntityTypeConfiguration<OperationClaim>
{
    public void Configure(EntityTypeBuilder<OperationClaim> builder)
    {
        builder.ToTable("OperationClaims").HasKey(oc => oc.Id);

        builder.Property(oc => oc.Id).HasColumnName("Id").IsRequired();
        builder.Property(oc => oc.OperationName).HasColumnName("OperationName").HasMaxLength(255).IsRequired();
        builder.Property(oc => oc.RequiredRoles).HasColumnName("RequiredRoles").HasMaxLength(500).IsRequired();

        builder.HasIndex(oc => oc.OperationName).IsUnique();

        builder.Property(oc => oc.CreatedTime).HasColumnName("CreatedTime").IsRequired();
        builder.Property(oc => oc.UpdateTime).HasColumnName("UpdateTime");

    }
}
