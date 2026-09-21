using TaskTracker.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Contracts;

using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;



namespace TaskTracker.Api;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    
    private readonly UserManager<ApplicationUser> _usermanager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _usermanager = userManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,        
        };

        var result = await _usermanager.CreateAsync(user, request.Password);

        if(!result.Succeeded) return BadRequest(result.Errors);

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Login(RegisterRequest request)
    {
        var user = await _usermanager.FindByEmailAsync(request.Email);
        if(user is null) return  Unauthorized();

        var passwordCorrect = await _usermanager.CheckPasswordAsync(user, request.Password);
        if(!passwordCorrect) return Unauthorized();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!)
        };

         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(2),
        signingCredentials: credentials
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Ok(new LoginResponse { Token = tokenString });


    }
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser(DeleteAccountDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _usermanager.FindByIdAsync(userId!);
        if(user is null) return Unauthorized();

        var isAuthorized = await _usermanager.CheckPasswordAsync(user, request.Password);
        if(!isAuthorized) return Unauthorized();

        var result = await _usermanager.DeleteAsync(user);
        if(!result.Succeeded) return BadRequest(result.Errors);
        return NoContent();
    }
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateUserPassword(ChangePasswordRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _usermanager.FindByIdAsync(userId!);
        if(user is null) return Unauthorized();

        var result = await _usermanager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if(!result.Succeeded) return Unauthorized(result.Errors);
        return Ok();

    }
    

    


}