using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Contracts;


namespace TaskTracker.IntegrationTests;

public class TaskItemEndpointsTests : TaskTrackerTestBase
{
    
    public TaskItemEndpointsTests(TaskTrackerApiFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task AddTask_ReturnOk()
    {
        // Arrange
        var token = await GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var boardId = await CreateBoard();

        var request = new {title = "Test Task", description = "test description", deadline = "2026-12-31", boardid = boardId.Id};


        //Act
        var response = await _client.PostAsJsonAsync("api/TaskItem", request);

        //Assert

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
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
        var boardId = await CreateBoard();
        var requests = new[]
{
    new { title = "Task 1", description = "första", deadline = "2026-01-01", boardid = boardId.Id },
    new { title = "Task 2", description = "andra", deadline = "2026-02-15", boardid = boardId.Id },
    new { title = "Task 3", description = "tredje", deadline = "2026-03-30", boardid = boardId.Id },
};
        foreach(var r in requests)
        {
            
            await _client.PostAsJsonAsync("/api/TaskItem", r);
        }
        
        
        //Act
        var response = await _client.GetAsync($"/api/board/{boardId!.Id}/task");

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tasks = await response.Content.ReadFromJsonAsync<List<TaskItemResponse>>();
        Assert.NotNull(tasks);
        Assert.Equal(3, tasks!.Count);
    }
    [Fact]
    public async Task GetById_ResponseOk()
    {
        // Arrange
        var token = await GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var boardId = await CreateBoard();
        var requests = new[]
        {
            new { title = "Task 1", description = "första", deadline = "2026-01-01", boardid = boardId.Id },
            new { title = "Task 2", description = "andra", deadline = "2026-02-15", boardid = boardId.Id },
            new { title = "Task 3", description = "tredje", deadline = "2026-03-30", boardid = boardId.Id },
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
        var task = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        Assert.NotNull(task);
        Assert.Equal(1, task.Id);
        Assert.Equal("Task 1", task!.Title);
    }

   
    
}