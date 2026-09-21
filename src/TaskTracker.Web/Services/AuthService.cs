using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskTracker.Contracts;

namespace TaskTracker.Web.Services;
public class AuthService
{
    private readonly HttpClient _client;
    public string? Token {get; private set;}
    public record AuthResult(bool Success, List<string> Errors);

    public record IdentityErrorDto(string Code, string Description);


    public AuthService(HttpClient client)
    {
        _client = client;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await _client.PostAsJsonAsync("/api/Auth", new {email, password});

        if(!response.IsSuccessStatusCode) return false;

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Token = body!.Token;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        return true;

    }

    public async Task<AuthResult> RegisterAsync(string email, string password)
    {
        var response = await _client.PostAsJsonAsync("api/Auth/register", new {email, password});

        if(response.IsSuccessStatusCode) return new AuthResult(true, new List<string>());

        var errors = await response.Content.ReadFromJsonAsync<List<IdentityErrorDto>>();
        return new AuthResult(false, errors?.Select(e => e.Description).ToList() ?? ["Registering misslyckades"]);
    }

    public async Task<AuthResult> DeleteAsync(string password)
    {
        
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, "api/Auth")
        {
            Content = JsonContent.Create(new {password})

        };

        var response = await _client.SendAsync(deleteRequest);
        if(response.IsSuccessStatusCode) return new AuthResult(true,new List<string>());

        var errors = await response.Content.ReadFromJsonAsync<List<IdentityErrorDto>>();
        return new AuthResult(false, errors?.Select(e => e.Description).ToList() ?? ["Borttagning misslyckades"]);
        
    }
    public async Task<AuthResult> UpdatePasswordAsync(string newPassword, string oldPassword)
    {
        var response = await _client.PutAsJsonAsync("api/Auth", new {newPassword, oldPassword});
        if(response.IsSuccessStatusCode) return new AuthResult(true,new List<string>());

        var errors = await response.Content.ReadFromJsonAsync<List<IdentityErrorDto>>();
        return new AuthResult(false, errors?.Select(e => e.Description).ToList() ?? ["Updatering av lösenord misslyckades"]);
    }


}