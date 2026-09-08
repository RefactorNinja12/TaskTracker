using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Contracts;

public class RegisterRequest
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password {get; set;} = string.Empty;
}