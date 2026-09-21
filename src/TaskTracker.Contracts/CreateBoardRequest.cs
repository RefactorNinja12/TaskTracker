namespace TaskTracker.Contracts;

public class CreateBoardRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description {get; set;} = string.Empty;
}