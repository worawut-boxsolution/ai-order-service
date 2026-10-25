using Microsoft.EntityFrameworkCore;
using OrderApi.Models;

namespace OrderApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<MasterBranch> Branches { get; set; }
    public DbSet<MasterJobType> JobTypes { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobPhoto> JobPhotos { get; set; }
    public DbSet<IssueLog> IssueLogs { get; set; }
    public DbSet<BillingExpense> BillingExpenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasIndex(u => u.Phone).IsUnique();
        
        // Seed initial data if necessary or handle configuration
    }
}
