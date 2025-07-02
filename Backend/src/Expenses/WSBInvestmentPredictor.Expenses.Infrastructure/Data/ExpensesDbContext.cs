using Microsoft.EntityFrameworkCore;
using WSBInvestmentPredictor.Expenses.Domain.Categorization;
using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Infrastructure.Data;

public class ExpensesDbContext : DbContext
{
    public ExpensesDbContext(DbContextOptions<ExpensesDbContext> options) : base(options)
    {
    }

    public DbSet<BankTransaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CategoryRule> CategoryRules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // BankTransaction configuration
        modelBuilder.Entity<BankTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Title).HasMaxLength(500);
            entity.Property(e => e.Account).HasMaxLength(100);
            entity.Property(e => e.Counterparty).HasMaxLength(200);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.Details).HasMaxLength(1000);
            entity.Property(e => e.TransactionNumber).HasMaxLength(50);
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.BlockedCurrency).HasMaxLength(3);
            entity.Property(e => e.PaymentCurrency).HasMaxLength(3);
            entity.Property(e => e.BalanceCurrency).HasMaxLength(3);
            
            entity.HasIndex(e => e.TransactionDate);
            entity.HasIndex(e => e.Account);
            entity.HasIndex(e => e.Counterparty);
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // CategoryRule configuration
        modelBuilder.Entity<CategoryRule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Keyword).HasMaxLength(200).IsRequired();
            
            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
} 