using TaskTracker.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Domain;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Contracts;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.Versioning;
namespace TaskTracker.Api;


[ApiController]
[Route("api/[controller]")]
[Authorize]

public class BoardController : ControllerBase
{
    private readonly AppDbContext _context;

    public BoardController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Board>>> GetAllBoardAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId is null) return Unauthorized();
        var boards = await _context.Boards.Where(x => x.UserId == userId).ToListAsync();
        return Ok(boards);   
    }
    [HttpGet("{id}")]

    public async Task<ActionResult<Board>> GetBoardByIdAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
       if(userId is null) return Unauthorized();
       var board = await _context.Boards.FindAsync(id);
       if(board is null) return NotFound();
       if(board.UserId != userId) return Forbid();
       return board;
    }
    [HttpPost]
    public async Task<ActionResult<Board>> CreateBoardAsync(CreateBoardRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId is null) return Unauthorized();
        var board = new Board
        {
           Name = request.Title,
           Description = request.Description,
           CreatedAt = DateTimeOffset.UtcNow,
           UserId = userId
        };
        _context.Add(board);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBoardByIdAsync), new {id = board.Id}, board);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBoardAsync(CreateBoardRequest request, int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId is null) return Unauthorized();
        var board = await _context.Boards.FindAsync(id);
        if(board is null) return NotFound();
        if(board.UserId != userId) return Forbid();
        board.Name = request.Title;
        board.Description = request.Description;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBoardAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId is null) return Unauthorized();
        var board = await _context.Boards.FindAsync(id);
        if(board is null) return NotFound();
        if(board.UserId != userId) return Forbid();
        _context.Boards.Remove(board);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
        
    
}