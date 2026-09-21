using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Contracts;

public class DeleteAccountDto
{
    public string Password { get; set; } = string.Empty;
}