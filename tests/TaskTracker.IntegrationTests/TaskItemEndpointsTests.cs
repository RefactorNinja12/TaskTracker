using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Contracts;
using TaskTracker.Domain;
using TaskTracker.Infrastructure;


namespace TaskTracker.IntegrationTests;

public class TaskItemEndpointsTests : IClassFixture<TaskTrackerApiFactory>
{
    private readonly HttpClient _client; 

    public TaskItemEndpointsTests(TaskTrackerApiFactory factory)
    {
        factory.ResetDatabase();
        _client = factory.CreateClient();
    }


    [Fact]
    public async Task AddTask_ReturnOk()
    {
        // Arrange
        var token = await GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var request = new {title = "Test Task", description = "test description", deadline = "2026-12-31"};

        //Act
        var response = await _client.PostAsJsonAsync("api/TaskItem", request);

        //Assert

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal("Test Task", created.Title);
        Assert.Equal("test description", created.Description);
        
    }

    [Fact]
    public async Task GetAll_ResponseOk()
    {
        //Arrange
        var token = await GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var request = new {title = "new test", description = "tests description", deadline = "2024-05-10"};
        for(int i = 0; i < 10; i++)
        {
            await _client.PostAsJsonAsync("/api/TaskItem", request);
        }
        //Act
        var response = await _client.GetAsync("/api/TaskItem");
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tasks = await response.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.NotNull(tasks);
        Assert.Equal(10, tasks!.Count);
    }
    [Fact]
    public async Task GetById_ResponseOk()
    {
        // Arrange
        var token = await GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var requests = new[]
        {
            new { title = "Task 1", description = "första", deadline = "2026-01-01" },
            new { title = "Task 2", description = "andra", deadline = "2026-02-15" },
            new { title = "Task 3", description = "tredje", deadline = "2026-03-30" },
        };

        foreach(var r in requests)
        {
            await _client.PostAsJsonAsync("/api/TaskItem", r);
        }

        var id = 1;

        //Act
        var response = await _client.GetAsync($"/api/TaskItem/{id}");

        // Assert 
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var task = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(task);
        Assert.Equal(1, task.Id);
        Assert.Equal("Task 1", task!.Title);
    }

    private async Task<string> GetToken()
    {
        await _client.PostAsJsonAsync("api/Auth/register", new {email = "login@example.com", password = "Test1234!"});
        var loginResponse = await _client.PostAsJsonAsync("api/Auth", new {email = "login@example.com", password = "Test1234!"});
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        var token = loginBody!.Token;
        return token; 
    }
    
}