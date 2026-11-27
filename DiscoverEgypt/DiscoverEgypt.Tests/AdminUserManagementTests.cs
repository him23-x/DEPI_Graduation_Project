using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.AdminUsers;
using DiscoverEgypt.Application.Enums.AdminUsers;
using DiscoverEgypt.Application.Features.AdminUsers.Handlers.Commands;
using DiscoverEgypt.Application.Features.AdminUsers.Requests.Commands;
using DiscoverEgypt.Application.Features.AdminUsers.Validators;
using DiscoverEgypt.Application.Responses.AdminUsers;
using DiscoverEgypt.Presentation.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Tests
{
    public sealed class AdminUserManagementTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Handler_ChangesOnlyTheRequestedActiveState(bool isActive)
        {
            var repository = new FakeRepository();
            var handler = new SetAdminUserActiveStateCommandHandler(repository, new SetAdminUserActiveStateCommandValidator());

            var response = await handler.Handle(new SetAdminUserActiveStateCommand(
                "admin-1", "user-2", new AdminUserStateDto { IsActive = isActive }), CancellationToken.None);

            Assert.Equal(AdminUserStateChangeOutcome.Success, response.Outcome);
            Assert.Equal("user-2", repository.UpdatedUserId);
            Assert.Equal(isActive, repository.UpdatedIsActive);
        }

        [Fact]
        public async Task Handler_ReturnsUnavailableWhenAccountDoesNotExist()
        {
            var repository = new FakeRepository { UserExists = false };
            var handler = new SetAdminUserActiveStateCommandHandler(repository, new SetAdminUserActiveStateCommandValidator());

            var response = await handler.Handle(new SetAdminUserActiveStateCommand(
                "admin-1", "missing-user", new AdminUserStateDto { IsActive = false }), CancellationToken.None);

            Assert.Equal(AdminUserStateChangeOutcome.UserUnavailable, response.Outcome);
        }

        [Fact]
        public async Task Validator_RequiresAdminAndTargetAccountIds()
        {
            var result = await new SetAdminUserActiveStateCommandValidator().ValidateAsync(
                new SetAdminUserActiveStateCommand("", "", new AdminUserStateDto()));

            Assert.False(result.IsValid);
        }

        [Fact]
        public void AccountManagement_RequiresAdminRoleAndAntiforgery()
        {
            var authorization = typeof(UsersController)
                .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
                .Cast<AuthorizeAttribute>().Single();
            var action = typeof(UsersController).GetMethod(nameof(UsersController.SetStatus));

            Assert.Equal("Admin", authorization.Roles);
            Assert.NotNull(action);
            Assert.NotEmpty(action!.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), inherit: true));
            Assert.Equal("IsActive", Assert.Single(typeof(AdminUserStateDto).GetProperties()).Name);
        }

        private sealed class FakeRepository : IAdminUserManagementRepository
        {
            public bool UserExists { get; init; } = true;
            public string? UpdatedUserId { get; private set; }
            public bool? UpdatedIsActive { get; private set; }

            public Task<IReadOnlyList<AdminUserListItemResponse>> GetAllAsync(CancellationToken cancellationToken = default) =>
                Task.FromResult<IReadOnlyList<AdminUserListItemResponse>>([]);

            public Task<bool> SetActiveStateAsync(string userId, bool isActive, CancellationToken cancellationToken = default)
            {
                UpdatedUserId = userId;
                UpdatedIsActive = isActive;
                return Task.FromResult(UserExists);
            }
        }
    }
}
