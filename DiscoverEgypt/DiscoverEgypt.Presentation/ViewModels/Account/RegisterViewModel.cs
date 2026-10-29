using System.ComponentModel.DataAnnotations;
using DiscoverEgypt.Application.Enums.Accounts;

namespace DiscoverEgypt.Presentation.ViewModels.Account;

public sealed class RegisterViewModel
{
    [Required, StringLength(150, MinimumLength = 2)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(8)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "I am joining as")]
    public UserRole? Role { get; set; }

    [Display(Name = "I agree to the Terms of Service and Privacy Policy")]
    public bool AcceptTerms { get; set; }
}
