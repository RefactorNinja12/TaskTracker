using TaskTracker.Domain;
namespace TaskTracker.Contracts;

public class TaskItemResponse
{
    
    public int Id { get; set; }
    public string Title {get; set;} = string.Empty;
    public string? Description {get; set;}  = string.Empty;
    public DateTimeOffset CreatedAt {get; set;}
    public DateOnly? Deadline {get; set;}
    public DateTimeOffset? Completed {get; set;}
    
    public TaskItemStatus Status {get; set;} = TaskItemStatus.ToDo;
    

}