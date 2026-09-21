namespace TaskTracker.Domain;

public class Board
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UserId {get; set;} = string.Empty;
    public DateTimeOffset CreatedAt {get; set;}
    public List<TaskItem> TaskItems { get; set; } = new();
    

}