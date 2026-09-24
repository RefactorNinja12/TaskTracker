using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using TaskTracker.Contracts;

namespace TaskTracker.Web.Services;

public class BoardService
{
    private readonly HttpClient _client; 
   
    public BoardService(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<BoardResponse>> GetBoards()
    {
        var response = await _client.GetFromJsonAsync<List<BoardResponse>>("/api/Board");
        return response ?? new List<BoardResponse>();
    }
    public async Task<BoardResponse?> CreateBoardAsync(string title, string description)
    {
        var response = await _client.PostAsJsonAsync("api/Board", new {title, description});
        if(!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<BoardResponse>();
    }
    public async Task<bool> DeleteBoard(int id)
    {
        
        var response = await _client.DeleteAsync($"/api/Board/{id}");
        return response.IsSuccessStatusCode; 
    }
    public async Task<bool> UpdateBoard(int id, string title, string description)
    {
        
        var response = await _client.PutAsJsonAsync($"/api/Board/{id}", new {title = title, description = description} );
        return response.IsSuccessStatusCode;
    }
}