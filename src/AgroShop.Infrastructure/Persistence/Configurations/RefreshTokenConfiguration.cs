using AgroShop.Domain.RefreshTokens;
using AgroShop.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroShop.Infrastructure.Persistence.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.TokenHash)
            .HasConversion(
                hash => hash.Value,
                value => TokenHash.Create(value))
            .IsRequired();

        builder.HasIndex(t => t.TokenHash)
            .IsUnique();

        builder.HasIndex(t => t.FamilyId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}