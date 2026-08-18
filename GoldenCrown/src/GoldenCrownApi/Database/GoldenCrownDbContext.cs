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

            userEntity.HasIndex(u => u.Login)
                .IsUnique();
            userEntity.HasMany(u => u.Accounts)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

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


            transactionEntity
                .HasOne(t => t.SenderAccount)
                .WithMany(a => a.SentTransactions)
                .HasForeignKey(t => t.SenderAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            transactionEntity
                .HasOne(t => t.ReceiverAccount)
                .WithMany(a => a.ReceivedTransactions)
                .HasForeignKey(t => t.ReceiverAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            sessionEntity
                .HasOne(s => s.User)
                .WithOne(u => u.Session)
                .HasForeignKey<Session>(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            accountEntity.HasOne(a => a.Currency)
                .WithMany(c => c.Accounts)
                .HasForeignKey(a => a.CurrencyId)
                .OnDelete(DeleteBehavior.NoAction);
            accountEntity.Property(a => a.CurrencyId)
                .HasDefaultValue(1);

            SeedUserData(currencyEntity);
        }

        private void SeedUserData(EntityTypeBuilder<Currency> currencyEntity)
        {
            currencyEntity.HasData(
                new Currency
                {
                    Id = 1,
                    Name = "RUB",
                },
                new Currency
                {
                    Id = 2,
                    Name = "USD",
                },
                new Currency
                {
                    Id = 3,
                    Name = "EUR",
                },
                new Currency
                {
                    Id = 4,
                    Name = "JPY",
                }
            );
        }
    }
}
