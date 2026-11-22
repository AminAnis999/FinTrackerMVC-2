using System.ComponentModel.DataAnnotations;

namespace FinTracker.Models;

public class RegisterViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, MinLength(8), DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required, Compare(nameof(Password)), DataType(DataType.Password), Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = "";
}
