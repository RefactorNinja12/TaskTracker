

namespace TaskTracker.Contracts;

public class BoardResponse
{
    public int Id {get; set;}
    public string Title { get; set; } = string.Empty;
    public string Description {get; set;} = string.Empty;
    public DateTimeOffset CreatedAt {get; set;}
    public int ToDoCount {get; set;}
    public int InProgressCount {get; set;}
    public int DoneCount {get; set;}
}