using System.Net;
using Api.Dtos.Users;
using Domain.UserRoles;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class UsersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly UserId _extraUserId = UserId.New();
    private readonly UserRole _mainUserRole = UserRolesData.MainUserRole();
    private readonly UserRoleId _extraUserRoleId = UserRoleId.New();

    public UsersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser(_mainUserRole.Id);
    }

    [Fact]
    public async Task ShouldCreateUser()
    {
        // Arrange
        var firstName = "Extra";
        var lastName = "User";
        var email = "5j3pO@example.com";
        var googleId = "googleId";
        var avatarUrl = "avatarUrl";

        var formData = new MultipartFormDataContent
        {
            { new StringContent(firstName), "firstName" },
            { new StringContent(lastName), "lastName" },
            { new StringContent(email), "email" },
            { new StringContent(googleId), "googleId" },
            { new StringContent(avatarUrl), "avatarUrl" },
        };
        // Act
        var response = await Client.PostAsync("/users/create", formData);
        var body = await response.Content.ReadAsStringAsync();
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<UserDto>();
        responseModel.Should().NotBeNull();
        responseModel!.FirstName.Should().Be(firstName);
        responseModel.LastName.Should().Be(lastName);
        responseModel.Email.Should().Be(email);
        responseModel.GoogleId.Should().Be(googleId);
        responseModel.AvatarUrl.Should().Be(avatarUrl);

        // Check if the user was created in the database
        var userId = new UserId(responseModel.Id);
        var userFromDatabase = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        userFromDatabase.Should().NotBeNull();
        userFromDatabase!.FirstName.Should().Be(firstName);
        userFromDatabase.LastName.Should().Be(lastName);
        userFromDatabase.Email.Should().Be(email);
        userFromDatabase.GoogleId.Should().Be(googleId);
        userFromDatabase.AvatarUrl.Should().Be(avatarUrl);
    }


    [Fact]
    public async Task ShouldNotCreateUser_Because_UserAlreadyExists()
    {
        // Arrange
        var existingUser = await Context.Users.FirstOrDefaultAsync();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(existingUser!.FirstName), "firstName" },
            { new StringContent(existingUser.LastName), "lastName" },
            { new StringContent(existingUser.Email), "email" },
            { new StringContent(existingUser.GoogleId), "googleId" },
            { new StringContent(existingUser.AvatarUrl), "avatarUrl" },
        };
        // Act
        var response = await Client.PostAsync("/users/create", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldUpdateUser()
    {
        // Arrange
        var firstName = "Hello";
        var lastName = "Worldovich";
        var avatarUrl = "avatarUrl";

        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainUser.Id.Value.ToString()), "id" },
            { new StringContent(firstName), "firstName" },
            { new StringContent(lastName), "lastName" },
            { new StringContent(avatarUrl), "avatarUrl" },
        };

        // Act
        var response = await Client.PutAsync("/users/update", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<UserDto>();
        responseModel.Should().NotBeNull();
        responseModel!.FirstName.Should().Be(firstName);
        responseModel.LastName.Should().Be(lastName);
        responseModel.AvatarUrl.Should().Be(avatarUrl);

        // Check if the user was updated in the database
        var userId = new UserId(responseModel.Id);
        var userFromDatabase = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        userFromDatabase.Should().NotBeNull();
        userFromDatabase!.FirstName.Should().Be(firstName);
        userFromDatabase.LastName.Should().Be(lastName);
        userFromDatabase.AvatarUrl.Should().Be(avatarUrl);
    }

    [Fact]
    public async Task ShouldNotUpdateUser_Because_UserNotFound()
    {
        // Arrange
        var nonExistentUserId = UserId.New();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(nonExistentUserId.Value.ToString()), "id" },
            { new StringContent("FirstName"), "firstName" },
            { new StringContent("LastName"), "lastName" },
            { new StringContent("avatarUrl"), "avatarUrl" },
        };

        // Act
        var response = await Client.PutAsync("/users/update", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldUpdateRoleForUser()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainUser.Id.Value.ToString()), "id" },
            { new StringContent(_extraUserRoleId.Value.ToString()), "userRoleId" },
        };

        // Act
        var response = await Client.PutAsync("/users/update-role", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<UserDto>();
        responseModel.Should().NotBeNull();
        responseModel!.UserRoleId.Should().Be(_extraUserRoleId.Value);

        // Check if the user was updated in the database
        var userId = new UserId(responseModel.Id);
        var userFromDatabase = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        userFromDatabase.Should().NotBeNull();
        userFromDatabase!.UserRoleId.Should().Be(_extraUserRoleId);
    }


    [Fact]
    public async Task ShouldNotUpdateRoleForUser_Because_UserRoleNotFound()
    {
        // Arrange
        var nonExistentUserRoleId = UserRoleId.New();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainUser.Id.Value.ToString()), "id" },
            { new StringContent(nonExistentUserRoleId.Value.ToString()), "userRoleId" },
        };

        // Act
        var response = await Client.PutAsync("/users/update-role", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateRoleForUser_Because_UserNotFound()
    {
        // Arrange
        var nonExistentUserId = UserId.New();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(nonExistentUserId.Value.ToString()), "id" },
            { new StringContent(_extraUserRoleId.Value.ToString()), "userRoleId" },
        };

        // Act
        var response = await Client.PutAsync("/users/update-role", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteUser()
    {
        // Arrange
        var userId = _mainUser.Id;

        // Act
        var response = await Client.DeleteAsync($"/users/delete?id={userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Check if the user was deleted from the database
        var userFromDatabase = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        userFromDatabase.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteUser_Because_UserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"/users/delete?id={userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.UserRoles.AddAsync(_mainUserRole);
        await Context.UserRoles.AddAsync(new UserRole(_extraUserRoleId, "User"));

        await Context.Users.AddAsync(_mainUser);
        await Context.Users.AddAsync(new User(_extraUserId, "Extra", "User", "example@example.com", "googleId", "avatarUrl", _extraUserRoleId, DateTime.UtcNow));

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.UserRoles.RemoveRange(Context.UserRoles);

        await SaveChangesAsync();
    }
}
