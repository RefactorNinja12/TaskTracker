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


    [HttpGet("/api/board/{boardId}/task")]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetAllTaskAsync(int boardId)
    {
         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
         if(userId is null) return Unauthorized();
         var board = await _context.Boards.FindAsync(boardId);
         if(board is null) return NotFound();
        if(board.UserId != userId) return Forbid();

        var tasks = await _context.Tasks.Where(t => t.BoardId == boardId).ToListAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetByIdAsync(int id)
    {
       var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
       if(userId is null) return Unauthorized();
       var task = await _context.Tasks.Include(t => t.Board).FirstOrDefaultAsync(t => t.Id == id);
        if(task is null) return NotFound();
        if(task.Board!.UserId != userId) return Forbid();

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTaskAsync(CreateTaskItemRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId is null) return Unauthorized();
        var board = await _context.Boards.FirstOrDefaultAsync(x => x.Id == request.BoardId);
        if(board is null) return NotFound();
        if(board.UserId != userId) return Forbid();

        var taskItem = new TaskItem
        {
           CreatedAt = DateTimeOffset.UtcNow,
           Title = request.Title,
           Description = request.Description,
           Deadline = request.Deadline,
           BoardId = request.BoardId
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
        
        var taskItem = await _context.Tasks.Include(t => t.Board).FirstOrDefaultAsync(t => t.Id == id);
        if(taskItem is null) return NotFound();

        if(taskItem.Board?.UserId != userId) return Forbid();

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

        var taskItem = await _context.Tasks.Include(t => t.Board).FirstOrDefaultAsync(t => t.Id == id);
        if(taskItem is null) return NotFound();

        if(taskItem.Board?.UserId != userId) return Forbid();
        _context.Tasks.Remove(taskItem);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    




}