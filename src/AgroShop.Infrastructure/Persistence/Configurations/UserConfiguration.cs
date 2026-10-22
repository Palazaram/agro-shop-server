using AgroShop.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroShop.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property(u => u.Phone)
            .HasConversion(
                phone => phone.Value,
                value => Phone.Create(value).Value)
            .HasMaxLength(Phone.MaxLength)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value).Value)
            .HasMaxLength(Email.MaxLength)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasConversion(
                hash => hash.Value,
                value => PasswordHash.Create(value))
            .IsRequired();

        builder.ComplexProperty(u => u.FullName, fullName =>
        {
            fullName.Property(n => n.FirstName)
                .HasConversion(
                    name => name.Value,
                    value => PersonName.Create(value).Value)
                .HasMaxLength(PersonName.MaxLength)
                .IsRequired();

            fullName.Property(n => n.LastName)
                .HasConversion(
                    name => name.Value,
                    value => PersonName.Create(value).Value)
                .HasMaxLength(PersonName.MaxLength)
                .IsRequired();

            fullName.Property(n => n.Patronymic)
                .HasConversion(
                    name => name.Value,
                    value => PersonName.Create(value).Value)
                .HasMaxLength(PersonName.MaxLength)
                .IsRequired();
        });

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.TermsAcceptedAt)
            .IsRequired();

        builder.Property(u => u.Role)
            .IsRequired();

        builder.HasIndex(u => u.Phone)
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.ToTable(t => t.HasCheckConstraint("ck_users_role", "role IN (1, 2)"));
    }
}