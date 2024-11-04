using System.Net;
using Api.Dtos.UsersAssignments;
using Domain.Assignments;
using Domain.Courses;
using Domain.Groups;
using Domain.Statuses;
using Domain.UserRoles;
using Domain.Users;
using Domain.UsersAssignments;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class UsersAssignmentsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly UserId _extraUserId = UserId.New();
    private readonly UserRole _mainUserRole = UserRolesData.MainUserRole();
    private readonly UserRoleId _extraUserRoleId = UserRoleId.New();
    private readonly Assignment _mainAssignment;
    private readonly AssignmentId _extraAssignmentId = AssignmentId.New();
    private readonly Course _mainCourse;
    private readonly CourseId _extraCourseId = CourseId.New();
    private readonly Group _mainGroup = GroupsData.MainGroup();
    private readonly GroupId _extraGroupId = GroupId.New();
    private readonly UserAssignment _mainUserAssignment;
    private readonly Status _mainStatus = StatusesData.MainStatus();
    private readonly StatusId _extraStatusId = StatusId.New();

    public UsersAssignmentsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser(_mainUserRole.Id);
        _mainCourse = CoursesData.MainCourse(_mainUser.Id, _mainGroup.Id);
        _mainAssignment = AssignmentsData.MainAssignment(_mainCourse.Id);
        _mainUserAssignment = UsersAssignmentsData.MainAssignment(_mainAssignment.Id, _mainUser.Id, _mainStatus.Id);
    }

    [Fact]
    public async Task ShouldCreateUserAssignment()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_extraAssignmentId.ToString()), "assignmentId" },
            { new StringContent(_extraUserId.ToString()), "userId" }
        };

        // Act
        var response = await Client.PostAsync("/users-assignments/create", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseModel = await response.ToResponseModel<UserAssignmentDto>();
        responseModel.Should().NotBeNull();
        responseModel!.AssignmentId.Should().Be(_extraAssignmentId.Value);
        responseModel.UserId.Should().Be(_extraUserId.Value);
        responseModel.StatusId.Should().Be(_mainStatus.Id.Value);

        // Check if the user assignment was created in the database
        var userId = _mainUser.Id;
        var assignmentId = _mainAssignment.Id;

        var userAssignmentFromDatabase = await Context.UserAssignments.FirstOrDefaultAsync(x => x.UserId == userId && x.AssignmentId == assignmentId);
        userAssignmentFromDatabase.Should().NotBeNull();
        userAssignmentFromDatabase!.AssignmentId.Should().Be(_mainAssignment.Id);
        userAssignmentFromDatabase.UserId.Should().Be(_mainUser.Id);
        userAssignmentFromDatabase.StatusId.Should().Be(_mainStatus.Id);
    }

    [Fact]
    public async Task ShouldNotCreateUserAssignment_Because_UserAssignmentAlreadyExists()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainUserAssignment.AssignmentId.Value.ToString()), "assignmentId" },
            { new StringContent(_mainUserAssignment.UserId.Value.ToString()), "userId" }
        };

        // Act
        var response = await Client.PostAsync("/users-assignments/create", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotCreateUserAssignment_Because_AssignmentNotFound()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(AssignmentId.New().ToString()), "assignmentId" },
            { new StringContent(_extraUserId.ToString()), "userId" }
        };

        // Act
        var response = await Client.PostAsync("/users-assignments/create", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotCreateUserAssignment_Because_UserNotFound()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_extraAssignmentId.ToString()), "assignmentId" },
            { new StringContent(UserId.New().ToString()), "userId" }
        };

        // Act
        var response = await Client.PostAsync("/users-assignments/create", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldUpdateUserAssignment()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainUserAssignment.AssignmentId.Value.ToString()), "assignmentId" },
            { new StringContent(_mainUserAssignment.UserId.Value.ToString()), "userId" },
            { new StringContent(_extraStatusId.Value.ToString()), "statusId" },
            { new StringContent(_mainUserAssignment.Score.ToString() ?? 0.ToString()), "maxScore" },
            { new StringContent(_mainUserAssignment.SubmittedAt.ToString() ?? DateTime.UtcNow.ToString()), "submittedAt" }
        };

        // Act
        var response = await Client.PutAsync("/users-assignments/update", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<UserAssignmentDto>();
        responseModel.Should().NotBeNull();
        responseModel!.AssignmentId.Should().Be(_mainUserAssignment.AssignmentId.Value);
        responseModel.UserId.Should().Be(_mainUserAssignment.UserId.Value);
        responseModel.StatusId.Should().Be(_extraStatusId.Value);

        // Check if the user assignment was updated in the database
        var userId = _mainUser.Id;
        var assignmentId = _mainAssignment.Id;

        var userAssignmentFromDatabase = await Context.UserAssignments.FirstOrDefaultAsync(x => x.UserId == userId && x.AssignmentId == assignmentId);
        userAssignmentFromDatabase.Should().NotBeNull();
        userAssignmentFromDatabase!.AssignmentId.Should().Be(_mainAssignment.Id);
        userAssignmentFromDatabase.UserId.Should().Be(_mainUser.Id);
        userAssignmentFromDatabase.StatusId.Should().Be(_extraStatusId);
    }


    [Fact]
    public async Task ShouldNotUpdateUserAssignment_Because_StatusNotFound()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainUserAssignment.AssignmentId.Value.ToString()), "assignmentId" },
            { new StringContent(_mainUserAssignment.UserId.Value.ToString()), "userId" },
            { new StringContent(Guid.NewGuid().ToString()), "statusId" },
            { new StringContent(_mainUserAssignment.Score.ToString() ?? 0.ToString()), "maxScore" },
            { new StringContent(_mainUserAssignment.SubmittedAt.ToString() ?? DateTime.UtcNow.ToString()), "submittedAt" }
        };

        // Act
        var response = await Client.PutAsync("/users-assignments/update", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateUserAssignment_Because_UserAssignmentNotFound()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(Guid.NewGuid().ToString()), "assignmentId" },
            { new StringContent(_mainUserAssignment.UserId.Value.ToString()), "userId" },
            { new StringContent(_extraStatusId.Value.ToString()), "statusId" },
            { new StringContent(_mainUserAssignment.Score.ToString() ?? 0.ToString()), "maxScore" },
            { new StringContent(_mainUserAssignment.SubmittedAt.ToString() ?? DateTime.UtcNow.ToString()), "submittedAt" }
        };

        // Act
        var response = await Client.PutAsync("/users-assignments/update", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteUserAssignment()
    {
        // Arrange & Act
        var response = await Client.DeleteAsync($"/users-assignments/delete?userId={_mainUser.Id.Value}&assignmentId={_mainAssignment.Id.Value}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Check if the user assignment was deleted from the database
        var userId = _mainUser.Id;
        var assignmentId = _mainAssignment.Id;

        var userAssignmentFromDatabase = await Context.UserAssignments.FirstOrDefaultAsync(x => x.UserId == userId && x.AssignmentId == assignmentId);
        userAssignmentFromDatabase.Should().BeNull();

        var userFromDatabase = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        userFromDatabase.Should().NotBeNull();

        var assignmentFromDatabase = await Context.Assignments.FirstOrDefaultAsync(x => x.Id == assignmentId);
        assignmentFromDatabase.Should().NotBeNull();

        var statusFromDatabase = await Context.Statuses.FirstOrDefaultAsync(x => x.Id == _mainStatus.Id);
        statusFromDatabase.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteUserAssignment_Because_UserAssignmentNotFound()
    {
        // Arrange
        var userId = UserId.New();
        var assignmentId = AssignmentId.New();

        // Act
        var response = await Client.DeleteAsync($"/users-assignments/delete?userId={userId.Value}&assignmentId={assignmentId.Value}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
    }

    public async Task InitializeAsync()
    {
        await Context.Statuses.AddAsync(_mainStatus);
        await Context.Statuses.AddAsync(new Status(_extraStatusId, "Extra status"));

        await Context.Groups.AddAsync(_mainGroup);
        await Context.Groups.AddAsync(new Group(_extraGroupId, "Extra group", "Extra group description", DateTime.UtcNow));

        await Context.UserRoles.AddAsync(_mainUserRole);
        await Context.UserRoles.AddAsync(new UserRole(_extraUserRoleId, "Extra user role"));

        await Context.Users.AddAsync(_mainUser);
        await Context.Users.AddAsync(new User(_extraUserId, "Extra", "User", "5j3pO@example.com", "googleId", "avatarUrl", _extraUserRoleId, DateTime.UtcNow));

        await Context.Courses.AddAsync(_mainCourse);
        await Context.Courses.AddAsync(new Course(_extraCourseId, "Extra course", "Extra course description", _extraUserId, _extraGroupId, DateTime.UtcNow));

        await Context.Assignments.AddAsync(_mainAssignment);
        await Context.Assignments.AddAsync(
            new Assignment(_extraAssignmentId, _extraCourseId, "Extra assignment", "Extra assignment description", DateTime.UtcNow.AddDays(7), 100m, DateTime.UtcNow));

        await Context.UserAssignments.AddAsync(_mainUserAssignment);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.UserAssignments.RemoveRange(Context.UserAssignments);
        Context.Assignments.RemoveRange(Context.Assignments);
        Context.Courses.RemoveRange(Context.Courses);
        Context.Users.RemoveRange(Context.Users);
        Context.UserRoles.RemoveRange(Context.UserRoles);
        Context.Groups.RemoveRange(Context.Groups);
        Context.Statuses.RemoveRange(Context.Statuses);

        await SaveChangesAsync();
    }
}
