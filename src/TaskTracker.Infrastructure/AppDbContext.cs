using System.Reflection.Metadata;
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
    public DbSet<Board> Boards {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<TaskItem>()
    .HasOne(t => t.Board)
    .WithMany(b => b.TaskItems)
    .HasForeignKey(t => t.BoardId)
    .IsRequired();

    modelBuilder.Entity<Board>()
        .HasOne<ApplicationUser>()
        .WithMany()
        .HasForeignKey(b => b.UserId)
        .IsRequired();
}
}