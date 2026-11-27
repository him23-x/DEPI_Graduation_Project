using DiscoverEgypt.Application.Enums.Accounts;

namespace DiscoverEgypt.Presentation.ViewModels.Account
{
    public sealed class RegisterViewModel
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        public UserRole? Role { get; set; }

        public bool AcceptTerms { get; set; }
    }
}

