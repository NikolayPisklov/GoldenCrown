using GoldenCrown.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenCrownApi.Database
{
    public class GoldenCrownDbContext : DbContext
    {
        public GoldenCrownDbContext(DbContextOptions<GoldenCrownDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var userEntity = modelBuilder.Entity<User>();
            var accountEntity = modelBuilder.Entity<Account>();
            var transactionEntity = modelBuilder.Entity<Transaction>();
            var sessionEntity = modelBuilder.Entity<Session>();
            var currencyEntity = modelBuilder.Entity<Currency>();

            userEntity.Property(u => u.Login)
                .IsRequired()
                .HasMaxLength(100);
            userEntity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(200);
            userEntity.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(256);
            userEntity.HasIndex(u => u.Login)
                .IsUnique();

            currencyEntity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(10);
            currencyEntity.HasIndex(u => u.Name)
                .IsUnique();

            accountEntity.HasIndex(a => new { a.UserId, a.CurrencyId })
                .IsUnique();
            accountEntity.Property(x => x.Balance)
                .HasPrecision(18, 2);
            accountEntity
                .ToTable(t => t.HasCheckConstraint("CK_Account_Balance_NonNegative", "[Balance] >= 0"));

            transactionEntity.Property(x => x.Amount)
                .HasPrecision(18, 2);

            transactionEntity
                .ToTable(t => t.HasCheckConstraint("CK_Transaction_Amount_GreaterThanZero", "[Amount] > 0"));

            sessionEntity
                .HasKey(s => s.UserId);
            sessionEntity.Property(s => s.Token)
                .IsRequired()
                .HasMaxLength(200);

            accountEntity
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            transactionEntity
                .HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.SenderAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            transactionEntity
                .HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.ReceiverAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            sessionEntity
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Session>(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            accountEntity
                .HasOne<Currency>()
                .WithMany()
                .HasForeignKey(a => a.CurrencyId)
                .OnDelete(DeleteBehavior.NoAction);

            SeedUserData(currencyEntity);
        }

        private void SeedUserData(EntityTypeBuilder<Currency> currencyEntity)
        {
            currencyEntity.HasData(
                Currency.Create(1, "RUB"),
                Currency.Create(2, "USD"),
                Currency.Create(3, "EUR"),
                Currency.Create(4, "JPY")
            );
        }
    }
}
