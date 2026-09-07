using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain;


namespace TaskTracker.Infrastructure; 

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<TaskItem> Tasks {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<TaskItem>()
        .HasOne<ApplicationUser>()
        .WithMany()
        .HasForeignKey(t => t.UserId)
        .IsRequired();
}
}