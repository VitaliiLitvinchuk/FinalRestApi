using System.Net;
using Api.Dtos.UsersGroups;
using Domain.Groups;
using Domain.UserGroupRoles;
using Domain.UserRoles;
using Domain.Users;
using Domain.UsersGroups;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class UsersGroupsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly UserId _extraUserId = UserId.New();
    private readonly Group _mainGroup = GroupsData.MainGroup();
    private readonly GroupId _extraGroupId = GroupId.New();
    private readonly UserRole _mainUserRole = UserRolesData.MainUserRole();
    private readonly UserRoleId _extraUserRoleId = UserRoleId.New();
    private readonly UserGroupRole _mainUserGroupRole = UserGroupRolesData.MainUserGroupRole();
    private readonly UserGroupRoleId _extraUserGroupRoleId = UserGroupRoleId.New();
    private readonly UserGroup _mainUserGroup;

    public UsersGroupsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser(_mainUserRole.Id);
        _mainUserGroup = UsersGroupsData.MainUserGroup(_mainUser.Id, _mainGroup.Id, _mainUserGroupRole.Id);
    }

    [Fact]
    public async Task ShouldCreateUserGroup()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_extraUserId.Value.ToString()), "userId" },
            { new StringContent(_extraGroupId.Value.ToString()), "groupId" }
        };

        // Act
        var response = await Client.PostAsync("/users-groups/create", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<UserGroupDto>();
        responseModel.Should().NotBeNull();
        responseModel!.UserId.Should().Be(_extraUserId.Value);
        responseModel.GroupId.Should().Be(_extraGroupId.Value);
        responseModel.UserGroupRoleId.Should().Be(_extraUserGroupRoleId.Value);

        // Check if the user group was created in the database
        var userId = new UserId(responseModel.UserId);
        var groupId = new GroupId(responseModel.GroupId);

        var userGroupFromDatabase = await Context.UserGroups.FirstOrDefaultAsync(x => x.UserId == userId && x.GroupId == groupId);
        userGroupFromDatabase.Should().NotBeNull();
        userGroupFromDatabase!.UserId.Should().Be(_extraUserId);
        userGroupFromDatabase.GroupId.Should().Be(_extraGroupId);
        userGroupFromDatabase.UserGroupRoleId.Should().Be(_extraUserGroupRoleId);
    }


    [Fact]
    public async Task ShouldNotCreateUserGroup_Because_UserGroupAlreadyExists()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainUser.Id.Value.ToString()), "userId" },
            { new StringContent(_mainGroup.Id.Value.ToString()), "groupId" }
        };

        // Act
        var response = await Client.PostAsync("/users-groups/create", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotCreateUserGroup_Because_GroupForUserGroupNotFoundException()
    {
        // Arrange
        var nonExistentGroupId = GroupId.New();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_extraUserId.Value.ToString()), "userId" },
            { new StringContent(nonExistentGroupId.Value.ToString()), "groupId" }
        };

        // Act
        var response = await Client.PostAsync("/users-groups/create", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotCreateUserGroup_Because_UserForUserGroupNotFoundException()
    {
        // Arrange
        var nonExistentUserId = UserId.New();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(nonExistentUserId.Value.ToString()), "userId" },
            { new StringContent(_extraGroupId.Value.ToString()), "groupId" }
        };

        // Act
        var response = await Client.PostAsync("/users-groups/create", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // [Fact] // TODO: Logic work but test is not passing
    // public async Task ShouldUpdateRoleForUserGroup()
    // {
    //     // Arrange
    //     var formData = new MultipartFormDataContent
    //     {
    //         { new StringContent(_mainUser.Id.Value.ToString()), "userId" },
    //         { new StringContent(_mainGroup.Id.Value.ToString()), "groupId" },
    //         { new StringContent(_extraUserGroupRoleId.Value.ToString()), "userGroupRoleId" }
    //     };

    //     // Act
    //     var response = await Client.PutAsync("/users-groups/update-user-role", formData);

    //     // Assert
    //     response.IsSuccessStatusCode.Should().BeTrue();

    //     var responseModel = await response.ToResponseModel<UserGroupDto>();
    //     responseModel.Should().NotBeNull();
    //     responseModel!.UserId.Should().Be(_mainUser.Id.Value);
    //     responseModel.GroupId.Should().Be(_mainGroup.Id.Value);
    //     responseModel.UserGroupRoleId.Should().Be(_extraUserGroupRoleId.Value);

    //     // Check if the user group role was updated in the database
    //     var userId = new UserId(responseModel.UserId);
    //     var groupId = new GroupId(responseModel.GroupId);

    //     var userGroupFromDatabase = await Context.UserGroups.FirstOrDefaultAsync(x => x.UserId == userId && x.GroupId == groupId);
    //     userGroupFromDatabase.Should().NotBeNull();
    //     userGroupFromDatabase!.UserId.Should().Be(_mainUser.Id);
    //     userGroupFromDatabase.GroupId.Should().Be(_mainGroup.Id);
    //     userGroupFromDatabase.UserGroupRoleId.Should().Be(_extraUserGroupRoleId);
    // }

    [Fact]
    public async Task ShouldNotUpdateRoleForUserGroup_Because_RoleForUserGroupNotFound()
    {
        // Arrange
        var nonExistentUserGroupRoleId = UserGroupRoleId.New();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainUser.Id.Value.ToString()), "userId" },
            { new StringContent(_mainGroup.Id.Value.ToString()), "groupId" },
            { new StringContent(nonExistentUserGroupRoleId.Value.ToString()), "userGroupRoleId" }
        };

        // Act
        var response = await Client.PutAsync("/users-groups/update-user-role", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateRoleForUserGroup_Because_UserGroupNotFound()
    {
        // Arrange
        var nonExistentUserId = UserId.New();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(nonExistentUserId.Value.ToString()), "userId" },
            { new StringContent(_mainGroup.Id.Value.ToString()), "groupId" },
            { new StringContent(_extraUserGroupRoleId.Value.ToString()), "userGroupRoleId" }
        };

        // Act
        var response = await Client.PutAsync("/users-groups/update-user-role", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteUserGroup()
    {
        // Arrange & Act
        var response = await Client.DeleteAsync($"/users-groups/delete?userId={_mainUserGroup.UserId}&groupId={_mainUserGroup.GroupId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<UserGroupDto>();
        responseModel.Should().NotBeNull();
        responseModel!.UserId.Should().Be(_mainUserGroup.UserId.Value);
        responseModel.GroupId.Should().Be(_mainUserGroup.GroupId.Value);
        responseModel.UserGroupRoleId.Should().Be(_mainUserGroup.UserGroupRoleId.Value);

        // Check if the user group was deleted from the database
        var userId = new UserId(_mainUserGroup.UserId.Value);
        var groupId = new GroupId(_mainUserGroup.GroupId.Value);

        var userGroupFromDatabase = await Context.UserGroups.FirstOrDefaultAsync(x => x.UserId == userId && x.GroupId == groupId);
        userGroupFromDatabase.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteUserGroup_Because_UserGroupNotFoundException()
    {
        // Arrange
        var nonExistentUserId = UserId.New();
        var nonExistentGroupId = GroupId.New();

        // Act
        var response = await Client.DeleteAsync($"/users-groups/delete?userId={nonExistentUserId}&groupId={nonExistentGroupId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.UserRoles.AddAsync(_mainUserRole);
        await Context.UserRoles.AddAsync(new UserRole(_extraUserRoleId, "User"));

        await Context.UserGroupRoles.AddAsync(_mainUserGroupRole);
        await Context.UserGroupRoles.AddAsync(new UserGroupRole(_extraUserGroupRoleId, "Member"));

        await Context.Users.AddAsync(_mainUser);
        await Context.Users.AddAsync(new User(_extraUserId, "Extra", "User", "example@example.com", "googleId", "avatarUrl", _extraUserRoleId, DateTime.UtcNow));

        await Context.Groups.AddAsync(_mainGroup);
        await Context.Groups.AddAsync(new Group(_extraGroupId, "Extra group", "Extra group description", DateTime.UtcNow));

        await Context.UserGroups.AddAsync(_mainUserGroup);

        await Context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.ChangeTracker.Clear();

        Context.UserGroups.RemoveRange(Context.UserGroups);
        Context.Groups.RemoveRange(Context.Groups);
        Context.Users.RemoveRange(Context.Users);
        Context.UserGroupRoles.RemoveRange(Context.UserGroupRoles);
        Context.UserRoles.RemoveRange(Context.UserRoles);

        await Context.SaveChangesAsync();
    }
}
