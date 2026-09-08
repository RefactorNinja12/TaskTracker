namespace TaskTracker.Contracts;  
public class CreateTaskItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public DateOnly? Deadline {get; set;}

}