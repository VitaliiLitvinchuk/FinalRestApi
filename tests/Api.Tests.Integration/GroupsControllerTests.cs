using System.Net;
using Api.Dtos.Groups;
using Domain.Groups;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class GroupsControllerTests(IntegrationTestWebFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
    private readonly Group _mainGroup = GroupsData.MainGroup();

    [Fact]
    public async Task ShouldCreateGroup()
    {
        // Arrange
        var name = "Test group";
        var description = "Test group description";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(name), "name" },
            { new StringContent(description), "description" },
        };

        // Act
        var response = await Client.PostAsync("/groups/create", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseModel = await response.ToResponseModel<GroupDto>();
        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(name);
        responseModel.Description.Should().Be(description);
    }

    [Fact]
    public async Task ShouldUpdateGroup()
    {
        // Arrange
        var name = "New group";
        var description = "New group description";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainGroup.Id.ToString()), "id" },
            { new StringContent(name), "name" },
            { new StringContent(description), "description" },
        };

        // Act
        var response = await Client.PutAsync("/groups/update", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseModel = await response.ToResponseModel<GroupDto>();
        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(name);
        responseModel.Description.Should().Be(description);

        // Check if the group was updated in the database
        var groupId = new GroupId(responseModel.Id);
        var groupFromDatabase = await Context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
        groupFromDatabase.Should().NotBeNull();
        groupFromDatabase!.Name.Should().Be(name);
        groupFromDatabase.Description.Should().Be(description);
    }

    [Fact]
    public async Task ShouldNotUpdateGroup_Because_GroupNotFound()
    {
        // Arrange
        var nonExistentGroupId = GroupId.New();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(nonExistentGroupId.Value.ToString()), "id" },
            { new StringContent("Updated Name"), "name" },
            { new StringContent("Updated Description"), "description" },
        };

        // Act
        var response = await Client.PutAsync("/groups/update", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteGroup()
    {
        // Arrange & Act
        var response = await Client.DeleteAsync($"/groups/delete?id={_mainGroup.Id}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<GroupDto>();
        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(_mainGroup.Name);

        // Check if the group was deleted from the database
        var groupId = new GroupId(responseModel.Id);
        var groupFromDatabase = await Context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
        groupFromDatabase.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteGroup_Because_GroupNotFound()
    {
        // Arrange
        var nonExistentGroupId = GroupId.New();

        // Act
        var response = await Client.DeleteAsync($"/groups/delete?id={nonExistentGroupId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.Groups.AddAsync(_mainGroup);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Groups.RemoveRange(Context.Groups);

        await SaveChangesAsync();
    }
}
