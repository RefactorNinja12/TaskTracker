using System.ComponentModel.Design;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Infrastructure;
namespace TaskTracker.IntegrationTests;

public class TaskTrackerApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DefaultConnection",
            "Host=db;Port=5432;Database=tasktracker_test;Username=tasktracker;Password=devpassword");
        builder.UseSetting("Jwt:Key", "en-test-nyckel-minst-32-tecken-lång-för-ci-och-lokalt");
        builder.UseSetting("Jwt:Issuer", "TaskTrackerApi");
        builder.UseSetting("Jwt:Audience", "TaskTrackerClient");

    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }
    
}