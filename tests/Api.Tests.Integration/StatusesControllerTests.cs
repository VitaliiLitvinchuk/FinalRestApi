using System.Net;
using Api.Dtos.Statuses;
using Domain.Statuses;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class StatusesControllerTests(IntegrationTestWebFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
    private readonly Status _mainStatus = StatusesData.MainStatus();

    [Fact]
    public async Task ShouldCreateStatus()
    {
        // Arrange
        string statusName = "In progress";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(statusName), "name" },
        };

        // Act
        var response = await Client.PostAsync("/statuses/create", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<StatusDto>();
        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(statusName);

        // Check if the status was created in the database
        var statusId = new StatusId(responseModel.Id);
        var statusFromDatabase = await Context.Statuses.FirstOrDefaultAsync(x => x.Id == statusId);
        statusFromDatabase.Should().NotBeNull();
        statusFromDatabase!.Name.Should().Be(statusName);
    }


    [Fact]
    public async Task ShouldNotCreateStatus_Because_StatusAlreadyExists()
    {
        // Arrange
        string statusName = "Not Started";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(statusName), "name" },
        };

        // Act
        var response = await Client.PostAsync("/statuses/create", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldUpdateStatus()
    {
        // Arrange
        string statusName = "In progress";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainStatus.Id.Value.ToString()), "id" },
            { new StringContent(statusName), "name" },
        };

        // Act
        var response = await Client.PutAsync($"/statuses/update", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<StatusDto>();
        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(statusName);

        // Check if the status was updated in the database
        var statusId = new StatusId(responseModel.Id);
        var statusFromDatabase = await Context.Statuses.FirstOrDefaultAsync(x => x.Id == statusId);
        statusFromDatabase.Should().NotBeNull();
        statusFromDatabase!.Name.Should().Be(statusName);
    }

    [Fact]
    public async Task ShouldNotUpdateStatus_Because_StatusAlreadyExists()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainStatus.Id.Value.ToString()), "id" },
            { new StringContent(_mainStatus.Name), "name" }
        };

        // Act
        var response = await Client.PutAsync("/statuses/update", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotUpdateStatus_Because_StatusNotFound()
    {
        // Arrange
        var nonExistentStatusId = StatusId.New();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(nonExistentStatusId.Value.ToString()), "id" },
            { new StringContent("Updated Name"), "name" }
        };

        // Act
        var response = await Client.PutAsync("/statuses/update", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteStatus()
    {
        // Arrange & Act
        var response = await Client.DeleteAsync($"/statuses/delete?id={_mainStatus.Id.Value}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<StatusDto>();
        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(_mainStatus.Name);

        // Check if the status was deleted from the database
        var statusId = new StatusId(responseModel.Id);
        var statusFromDatabase = await Context.Statuses.FirstOrDefaultAsync(x => x.Id == statusId);
        statusFromDatabase.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteStatus_Because_StatusNotFound()
    {
        // Arrange
        var nonExistentStatusId = StatusId.New();

        // Act
        var response = await Client.DeleteAsync($"/statuses/delete?id={nonExistentStatusId.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.Statuses.AddAsync(_mainStatus);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Statuses.RemoveRange(Context.Statuses);

        await SaveChangesAsync();
    }
}
