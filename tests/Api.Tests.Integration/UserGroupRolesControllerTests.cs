using System.Net;
using Api.Dtos.UserGroupRoles;
using Domain.UserGroupRoles;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class UserGroupRolesControllerTests(IntegrationTestWebFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
    private readonly UserGroupRole _mainUserGroupRole = UserGroupRolesData.MainUserGroupRole();

    [Fact]
    public async Task ShouldCreateUserGroupRole()
    {
        // Arrange
        string roleName = "User";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(roleName), "name" },
        };

        // Act
        var response = await Client.PostAsync("/user-group-roles/create", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<UserGroupRoleDto>();
        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(roleName);

        // Check if the user role was created in the database
        var userGroupRoleId = new UserGroupRoleId(responseModel.Id);
        var userGroupRoleFromDatabase = await Context.UserGroupRoles.FirstOrDefaultAsync(x => x.Id == userGroupRoleId);
        userGroupRoleFromDatabase.Should().NotBeNull();
        userGroupRoleFromDatabase!.Name.Should().Be(roleName);
    }


    [Fact]
    public async Task ShouldNotCreateUserGroupRole_Because_UserGroupRoleAlreadyExists()
    {
        // Arrange
        string roleName = "Leader";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(roleName), "name" },
        };

        // Act
        var response = await Client.PostAsync("/user-group-roles/create", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldUpdateUserGroupRole()
    {
        // Arrange
        string roleName = "User";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainUserGroupRole.Id.Value.ToString()), "id" },
            { new StringContent(roleName), "name" },
        };

        // Act
        var response = await Client.PutAsync($"/user-group-roles/update", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<UserGroupRoleDto>();
        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(roleName);

        // Check if the user role was updated in the database
        var userGroupRoleId = new UserGroupRoleId(responseModel.Id);
        var userGroupRoleFromDatabase = await Context.UserGroupRoles.FirstOrDefaultAsync(x => x.Id == userGroupRoleId);
        userGroupRoleFromDatabase.Should().NotBeNull();
        userGroupRoleFromDatabase!.Name.Should().Be(roleName);
    }

    [Fact]
    public async Task ShouldNotUpdateUserGroupRole_Because_UserGroupRoleAlreadyExists()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainUserGroupRole.Id.Value.ToString()), "id" },
            { new StringContent(_mainUserGroupRole.Name), "name" }
        };

        // Act
        var response = await Client.PutAsync("/user-group-roles/update", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotUpdateUserGroupRole_Because_UserGroupRoleNotFound()
    {
        // Arrange
        var nonExistentUserGroupRoleId = UserGroupRoleId.New();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(nonExistentUserGroupRoleId.Value.ToString()), "id" },
            { new StringContent("Updated Name"), "name" }
        };

        // Act
        var response = await Client.PutAsync("/user-group-roles/update", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteUserGroupRole()
    {
        // Arrange & Act
        var response = await Client.DeleteAsync($"/user-group-roles/delete?id={_mainUserGroupRole.Id.Value}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<UserGroupRoleDto>();
        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(_mainUserGroupRole.Name);

        // Check if the user role was deleted from the database
        var userGroupRoleId = new UserGroupRoleId(responseModel.Id);
        var userGroupRoleFromDatabase = await Context.UserGroupRoles.FirstOrDefaultAsync(x => x.Id == userGroupRoleId);
        userGroupRoleFromDatabase.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteUserGroupRole_Because_UserGroupRoleNotFound()
    {
        // Arrange
        var nonExistentUserGroupRoleId = UserGroupRoleId.New();

        // Act
        var response = await Client.DeleteAsync($"/user-group-roles/delete?id={nonExistentUserGroupRoleId.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.UserGroupRoles.AddAsync(_mainUserGroupRole);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.UserGroupRoles.RemoveRange(Context.UserGroupRoles);

        await SaveChangesAsync();
    }
}
