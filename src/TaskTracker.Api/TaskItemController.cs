using TaskTracker.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Domain;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Contracts;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
namespace TaskTracker.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class TaskItemController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaskItemController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetAllTaskAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId is null) return Unauthorized();
        var tasks = _context.Tasks.Where(t => t.UserId == userId);
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetByIdAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId is null) return Unauthorized();
        var task = await _context.Tasks.FindAsync(id);
        if(task is null) return NotFound();
        if(task.UserId != userId) return Forbid();
        

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTaskAsync(CreateTaskItemRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId is null) return Unauthorized();

        var taskItem = new TaskItem
        {
           CreatedAt = DateTimeOffset.UtcNow,
           Title = request.Title,
           Description = request.Description,
           Deadline = request.Deadline,
           UserId = userId 
        };

        _context.Add(taskItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByIdAsync), new { id = taskItem.Id }, taskItem);
            
    }
   
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTaskItem(int id, UpdateTaskItemRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId is null) return Unauthorized();
        
        var taskItem = await _context.Tasks.FindAsync(id);
        if(taskItem is null) return NotFound();

        if(taskItem.UserId != userId) return Forbid();

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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId is null) return Unauthorized();

        var taskItem = await _context.Tasks.FindAsync(id);
        if(taskItem is null) return NotFound();

        if(taskItem.UserId != userId) return Forbid();
        _context.Tasks.Remove(taskItem);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    




}