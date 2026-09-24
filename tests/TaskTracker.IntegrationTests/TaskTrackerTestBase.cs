using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskTracker.Contracts;

namespace TaskTracker.IntegrationTests;

public abstract class TaskTrackerTestBase : IClassFixture<TaskTrackerApiFactory>
{
    protected readonly HttpClient _client;
    public TaskTrackerTestBase(TaskTrackerApiFactory factory)
    {
        factory.ResetDatabase();
        _client = factory.CreateClient();
    }

    protected async Task<string> GetToken()
    {
        await _client.PostAsJsonAsync("api/Auth/register", new { email = "login@example.com", password = "Test1234!" });
        var loginResponse = await _client.PostAsJsonAsync("api/Auth", new { email = "login@example.com", password = "Test1234!" });
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        return loginBody!.Token;
    }

    protected async Task<BoardResponse> CreateBoard()
    {
        var boardRequest = new { title = "TestTitle", description = "Det här är ett test" };
        var boardResponse = await _client.PostAsJsonAsync("/api/Board", boardRequest);
        var board = await boardResponse.Content.ReadFromJsonAsync<BoardResponse>();
        return board!;
    }
}