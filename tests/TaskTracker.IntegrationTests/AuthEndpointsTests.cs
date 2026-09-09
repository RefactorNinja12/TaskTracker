using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Contracts;
using TaskTracker.Infrastructure;


namespace TaskTracker.IntegrationTests;

public class AuthEndpointsTests : IClassFixture<TaskTrackerApiFactory >
{
    private readonly HttpClient _client;
    

    public AuthEndpointsTests(TaskTrackerApiFactory factory)
    {
        factory.ResetDatabase();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOk()
    {
        var request = new { email = "test@example.com", password = "Test1234!" };

        var response = await _client.PostAsJsonAsync("/api/Auth/register", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithMissingEmail_ReturnsBadRequest()
    {
        var request = new { email = "", password = "Test1234!" };

        var response = await _client.PostAsJsonAsync("/api/Auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        await _client.PostAsJsonAsync("/api/Auth/register",
            new { email = "login@example.com", password = "Test1234!" });

        var response = await _client.PostAsJsonAsync("/api/Auth",
            new { email = "login@example.com", password = "Test1234!" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.False(string.IsNullOrEmpty(body!.Token));
    }

    [Fact]
    public async Task DeleteAccount_NoContentRequest()
    {

        // arrange 
      await _client.PostAsJsonAsync("/api/Auth/register", new {email = "login@example.com", password = "Test1234!"});
      var loginResponse = await _client.PostAsJsonAsync("/api/Auth", new {email = "login@example.com", password = "Test1234!"});
      var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
      var token = loginBody!.Token;

      var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/Auth")
      {
         Content = JsonContent.Create(new { email = "login@example.com", password = "Test1234!" })
      };
      deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

    // act

    var response = await _client.SendAsync(deleteRequest);

    // Assert

    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    }

    [Fact]
    public async Task UpdatePassword_OkRequest()
    {
        //Arrange
        await _client.PostAsJsonAsync("api/Auth/register", new {email = "login@example.com", password = "Test1234!"});
        var loginResponse = await _client.PostAsJsonAsync("api/Auth", new {email = "login@example.com", password = "Test1234!"});
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        var token = loginBody!.Token;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var request = new {newpassword = "Test12345!", oldpassword = "Test1234!"};
        // Act
        var updateResponse = await _client.PutAsJsonAsync("api/Auth", request);
        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
    }
}
