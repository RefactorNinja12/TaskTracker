
namespace TaskTracker.Domain; 

public class TaskItem
{
    
    public int Id { get; set; }
    public string Title {get; set;} = string.Empty;
    public string? Description {get; set;}  = string.Empty;
    public DateTimeOffset CreatedAt {get; set;}
    public DateOnly? Deadline {get; set;}
    public DateTimeOffset? Completed {get; set;}
    public int BoardId{get; set;}
    public TaskItemStatus Status {get; set;} = TaskItemStatus.ToDo;
    public Board? Board { get; set; }

}