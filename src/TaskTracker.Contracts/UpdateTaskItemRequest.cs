using TaskTracker.Domain;
namespace TaskTracker.Contracts;

public class UpdateTaskItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public DateOnly? Deadline {get; set;}
    public DateTimeOffset? Completed {get; set;}
    public TaskItemStatus Status {get; set;} = TaskItemStatus.ToDo;
}