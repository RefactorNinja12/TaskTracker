using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using TaskTracker.Contracts;
namespace TaskTracker.Web.Services;



public class TaskItemService
{
    private readonly HttpClient _client; 

    public TaskItemService(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<TaskItemResponse>> GetTasks(int boardId)
    {
        var response = await _client.GetFromJsonAsync<List<TaskItemResponse>>($"/api/board/{boardId}/task");
        return response ?? new List<TaskItemResponse>();
    }
    public async Task<TaskItemResponse?> CreateTask(CreateTaskItemRequest request)
    {
        var response = await _client.PostAsJsonAsync("api/TaskItem", request);
        if(!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<TaskItemResponse>();
    }
    public async Task<bool> DeleteTaskAsync(int id)
    {
         var response = await _client.DeleteAsync($"/api/TaskItem/{id}");
         return response.IsSuccessStatusCode;
    }
    public async Task<bool> UpdateTaskAsync(int id, UpdateTaskItemRequest request)
    {
        var response = await _client.PutAsJsonAsync($"api/TaskItem/{id}", request);
        return response.IsSuccessStatusCode;
    }
    

}