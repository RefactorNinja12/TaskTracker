using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Contracts;

public class ChangePasswordRequest
{
    [Required]
    public string NewPassword { get; set; } = string.Empty;
    [Required]
    public string OldPassword { get; set; } = string.Empty;
}