using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskTracker.Contracts;
using TaskTracker.Domain;

namespace TaskTracker.IntegrationTests;

public class BoardEndpointsTests : TaskTrackerTestBase
{
    public BoardEndpointsTests(TaskTrackerApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateBoard_ReturnsCreated()
    {
        // Arrange
        var token = await GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var request = new { title = "My Board", description = "A test board" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Board", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<BoardResponse>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal("My Board", created.Title);
        Assert.Equal("A test board", created.Description);
    }

    [Fact]
    public async Task GetAllBoards_ReturnsOnlyOwnBoards()
    {
        // Arrange
        var token = await GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await CreateBoard();
        await CreateBoard();
        await CreateBoard();

        // Act
        var response = await _client.GetAsync("/api/Board");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var boards = await response.Content.ReadFromJsonAsync<List<BoardResponse>>();
        Assert.NotNull(boards);
        Assert.Equal(3, boards!.Count);
    }

    [Fact]
    public async Task GetBoardById_ReturnsOk()
    {
        // Arrange
        var token = await GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var board = await CreateBoard();

        // Act
        var response = await _client.GetAsync($"/api/Board/{board.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var fetched = await response.Content.ReadFromJsonAsync<BoardResponse>();
        Assert.NotNull(fetched);
        Assert.Equal(board.Id, fetched!.Id);
    }

    [Fact]
    public async Task UpdateBoard_ReturnsNoContent()
    {
        // Arrange
        var token = await GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var board = await CreateBoard();
        var updateRequest = new { title = "Updated Title", description = "Updated description" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Board/{board.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/Board/{board.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<BoardResponse>();
        Assert.Equal("Updated Title", updated!.Title);
        Assert.Equal("Updated description", updated.Description);
    }

    [Fact]
    public async Task DeleteBoard_ReturnsNoContent()
    {
        // Arrange
        var token = await GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var board = await CreateBoard();

        // Act
        var response = await _client.DeleteAsync($"/api/Board/{board.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/Board/{board.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetBoardById_OtherUsersBoard_ReturnsForbidden()
    {
        // Arrange: User A creates a board
        var tokenA = await GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenA);
        var board = await CreateBoard();

        // User B logs in
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenForOtherUser("userb1@example.com"));

        // Act
        var response = await _client.GetAsync($"/api/Board/{board.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBoard_OtherUsersBoard_ReturnsForbidden()
    {
        // Arrange: User A creates a board
        var tokenA = await GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenA);
        var board = await CreateBoard();

        // User B logs in
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenForOtherUser("userb2@example.com"));

        var updateRequest = new { title = "Hacked", description = "Hacked" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Board/{board.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBoard_OtherUsersBoard_ReturnsForbidden()
    {
        // Arrange: User A creates a board
        var tokenA = await GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenA);
        var board = await CreateBoard();

        // User B logs in
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenForOtherUser("userb3@example.com"));

        // Act
        var response = await _client.DeleteAsync($"/api/Board/{board.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<string> GetTokenForOtherUser(string email)
    {
        await _client.PostAsJsonAsync("api/Auth/register", new { email, password = "Test1234!" });
        var loginResponse = await _client.PostAsJsonAsync("api/Auth", new { email, password = "Test1234!" });
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        return loginBody!.Token;
    }
}
