using DiscoverEgypt.Application.Contracts.Identity;
using DiscoverEgypt.Application.Enums.Accounts;
using DiscoverEgypt.Application.Features.Accounts.Handlers;
using DiscoverEgypt.Application.Features.Accounts.Requests;
using DiscoverEgypt.Application.Features.Accounts.Validators;
using DiscoverEgypt.Application.Responses.Accounts;
using FluentValidation;

namespace DiscoverEgypt.Tests;

public sealed class AccountIdentityBoundaryTests
{
    [Fact]
    public async Task LoginHandler_DelegatesIdentityWorkAndReturnsItsResponse()
    {
        var identity = new FakeAccountIdentityService();
        var handler = new LoginAccountCommandHandler(identity, new LoginAccountCommandValidator());
        var command = new LoginAccountCommand("tourist@example.com", "correct-password", true);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Same(identity.LoginResponse, result);
        Assert.Equal(command.Email, identity.LoginEmail);
        Assert.Equal(command.Password, identity.LoginPassword);
        Assert.Equal(command.RememberMe, identity.LoginRememberMe);
    }

    [Fact]
    public async Task RegisterHandler_RejectsAdminBeforeCallingIdentityService()
    {
        var identity = new FakeAccountIdentityService();
        var handler = new RegisterAccountCommandHandler(identity, new RegisterAccountCommandValidator());
        var command = new RegisterAccountCommand("Admin User", "admin@example.com", "StrongPass1!", "StrongPass1!", UserRole.Admin, true);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(0, identity.RegisterCalls);
    }

    [Fact]
    public async Task RegisterHandler_DelegatesAllowedRoleToIdentityService()
    {
        var identity = new FakeAccountIdentityService();
        var handler = new RegisterAccountCommandHandler(identity, new RegisterAccountCommandValidator());
        var command = new RegisterAccountCommand("Test Host", "host@example.com", "StrongPass1!", "StrongPass1!", UserRole.Host, true);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Same(identity.RegisterResponse, result);
        Assert.Equal(1, identity.RegisterCalls);
        Assert.Equal(UserRole.Host, identity.RegisterRole);
    }

    private sealed class FakeAccountIdentityService : IAccountIdentityService
    {
        public AccountAuthenticationResponse LoginResponse { get; } = new(true, true, []);
        public AccountAuthenticationResponse RegisterResponse { get; } = new(true, false, []);
        public string? LoginEmail { get; private set; }
        public string? LoginPassword { get; private set; }
        public bool LoginRememberMe { get; private set; }
        public int RegisterCalls { get; private set; }
        public UserRole? RegisterRole { get; private set; }

        public Task<AccountAuthenticationResponse> LoginAsync(
            string email,
            string password,
            bool rememberMe,
            CancellationToken cancellationToken = default)
        {
            LoginEmail = email;
            LoginPassword = password;
            LoginRememberMe = rememberMe;
            return Task.FromResult(LoginResponse);
        }

        public Task<AccountAuthenticationResponse> RegisterAsync(
            string email,
            string fullName,
            string password,
            UserRole role,
            CancellationToken cancellationToken = default)
        {
            RegisterCalls++;
            RegisterRole = role;
            return Task.FromResult(RegisterResponse);
        }
    }
}
