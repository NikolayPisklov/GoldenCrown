using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Models
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

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<Account>().Property(x => x.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>().Property(x => x.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Account>()
                .ToTable(t => t.HasCheckConstraint("CK_Account_Balance_NonNegative", "[Balance] >= 0"));

            modelBuilder.Entity<Transaction>()
                .ToTable(t => t.HasCheckConstraint("CK_Transaction_Amount_GreaterThanZero", "[Amount] > 0"));

            modelBuilder.Entity<Session>()
                .HasKey(s => s.UserId);


            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.SenderAccount)
                .WithMany(a => a.SentTransactions)
                .HasForeignKey(t => t.SenderAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ReceiverAccount)
                .WithMany(a => a.ReceivedTransactions)
                .HasForeignKey(t => t.ReceiverAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            // Do I need to configure on_delete/on_update behavior for relationships?
        }
    }
}
