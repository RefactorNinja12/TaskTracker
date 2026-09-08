using TaskTracker.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Domain;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Contracts;
namespace TaskTracker.Api;

[ApiController]
[Route("api/[controller]")]

public class TaskItemController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaskItemController(AppDbContext context)
    {
        _context = context;
    }

    // Get

    [HttpGet]

    public async Task<ActionResult<IEnumerable<TaskItem>>> GetAllTaskAsync()
    {
        return await _context.Tasks.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetByIdAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if(task is null) return NotFound();

        return Ok(task);
    }

    /*
    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTaskAsync(CreateTaskItemRequest request)
    {
        var rand = new Random();
        
        
        
        var taskItem = new TaskItem
        {
           CreatedAt = DateTimeOffset.UtcNow,
           Title = request.Title,
           Description = request.Description,
           Deadline = request.Deadline,
           //  fix when register is up
           UserId =, 
        

        };

        _context.Add(taskItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByIdAsync), new { id = taskItem.Id }, taskItem);
            
    }
    */
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTaskItem(int id, UpdateTaskItemRequest request)
    {
        var taskItem = await _context.Tasks.FindAsync(id);
        if(taskItem is null) return NotFound();

        taskItem.Description = request.Description;
        taskItem.Completed = request.Completed;
        taskItem.Deadline = request.Deadline;
        taskItem.Status = request.Status;
        taskItem.Title = request.Title;

        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTaskItem(int id)
    {
        var taskItem = await _context.Tasks.FindAsync(id);
        if(taskItem is null) return NotFound();

        _context.Tasks.Remove(taskItem);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    




}