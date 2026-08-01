using GoldenCrown.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenCrown.Database
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var userEntity = modelBuilder.Entity<User>();
            var accountEntity = modelBuilder.Entity<Account>();
            var transactionEntity = modelBuilder.Entity<Transaction>();
            var sessionEntity = modelBuilder.Entity<Session>();

            userEntity.HasIndex(u => u.Login)
                .IsUnique();

            accountEntity.Property(x => x.Balance)
                .HasPrecision(18, 2);

            transactionEntity.Property(x => x.Amount)
                .HasPrecision(18, 2);

            accountEntity
                .ToTable(t => t.HasCheckConstraint("CK_Account_Balance_NonNegative", "[Balance] >= 0"));

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

            accountEntity
                .HasOne(s => s.User)
                .WithOne(u => u.Account)
                .HasForeignKey<Account>(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            SeedUserData(userEntity);
        }

        private void SeedUserData(EntityTypeBuilder<User> userEntity)
        {
            userEntity.HasData(
                new User
                {
                    Id = 1,
                    Login = "admin",
                    Name = "Administrator",
                    Password = "admin123"
                },
                new User
                {
                    Id = 2,
                    Login = "ivan",
                    Name = "Ivan Petrov",
                    Password = "ivan123"
                },
                new User
                {
                    Id = 3,
                    Login = "maria",
                    Name = "Maria Smirnova",
                    Password = "maria123"
                },
                new User
                {
                    Id = 4,
                    Login = "alex",
                    Name = "Alex Kuznetsov",
                    Password = "alex123"
                },
                new User
                {
                    Id = 5,
                    Login = "elena",
                    Name = "Elena Sokolova",
                    Password = "elena123"
                }
            );
        }
    }
}
