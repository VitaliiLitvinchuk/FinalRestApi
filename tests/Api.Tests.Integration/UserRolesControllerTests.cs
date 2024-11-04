using System.Net;
using Api.Dtos.UserRoles;
using Domain.UserRoles;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class UserRolesControllerTests(IntegrationTestWebFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
    private readonly UserRole _mainUserRole = UserRolesData.MainUserRole();

    [Fact]
    public async Task ShouldCreateUserRole()
    {
        // Arrange
        string roleName = "User";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(roleName), "name" },
        };

        // Act
        var response = await Client.PostAsync("/user-roles/create", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<UserRoleDto>();
        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(roleName);

        // Check if the user role was created in the database
        var userRoleId = new UserRoleId(responseModel.Id);
        var userRoleFromDatabase = await Context.UserRoles.FirstOrDefaultAsync(x => x.Id == userRoleId);
        userRoleFromDatabase.Should().NotBeNull();
        userRoleFromDatabase!.Name.Should().Be(roleName);
    }


    [Fact]
    public async Task ShouldNotCreateUserRole_Because_UserRoleAlreadyExists()
    {
        // Arrange
        string roleName = "Admin";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(roleName), "name" },
        };

        // Act
        var response = await Client.PostAsync("/user-roles/create", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldUpdateUserRole()
    {
        // Arrange
        string roleName = "User";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainUserRole.Id.Value.ToString()), "id" },
            { new StringContent(roleName), "name" },
        };

        // Act
        var response = await Client.PutAsync($"/user-roles/update", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<UserRoleDto>();
        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(roleName);

        // Check if the user role was updated in the database
        var userRoleId = new UserRoleId(responseModel.Id);
        var userRoleFromDatabase = await Context.UserRoles.FirstOrDefaultAsync(x => x.Id == userRoleId);
        userRoleFromDatabase.Should().NotBeNull();
        userRoleFromDatabase!.Name.Should().Be(roleName);
    }

    [Fact]
    public async Task ShouldNotUpdateUserRole_Because_UserRoleAlreadyExists()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainUserRole.Id.Value.ToString()), "id" },
            { new StringContent(_mainUserRole.Name), "name" }
        };

        // Act
        var response = await Client.PutAsync("/user-roles/update", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotUpdateUserRole_Because_UserRoleNotFound()
    {
        // Arrange
        var nonExistentUserRoleId = UserRoleId.New();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(nonExistentUserRoleId.Value.ToString()), "id" },
            { new StringContent("Updated Name"), "name" }
        };

        // Act
        var response = await Client.PutAsync("/user-roles/update", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteUserRole()
    {
        // Arrange & Act
        var response = await Client.DeleteAsync($"/user-roles/delete?id={_mainUserRole.Id.Value}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<UserRoleDto>();
        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(_mainUserRole.Name);

        // Check if the user role was deleted from the database
        var userRoleId = new UserRoleId(responseModel.Id);
        var userRoleFromDatabase = await Context.UserRoles.FirstOrDefaultAsync(x => x.Id == userRoleId);
        userRoleFromDatabase.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteUserRole_Because_UserRoleNotFound()
    {
        // Arrange
        var nonExistentUserRoleId = UserRoleId.New();

        // Act
        var response = await Client.DeleteAsync($"/user-roles/delete?id={nonExistentUserRoleId.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.UserRoles.AddAsync(_mainUserRole);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.UserRoles.RemoveRange(Context.UserRoles);

        await SaveChangesAsync();
    }
}
