using System.Net;
using Api.Dtos.Assignments;
using Domain.Assignments;
using Domain.Courses;
using Domain.Groups;
using Domain.UserRoles;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class AssignmentsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Assignment _mainAssignment;
    private readonly AssignmentId _extraAssignmentId = AssignmentId.New();
    private readonly Course _mainCourse;
    private readonly CourseId _extraCourseId = CourseId.New();
    private readonly User _mainUser;
    private readonly UserId _extraUserId = UserId.New();
    private readonly Group _mainGroup = GroupsData.MainGroup();
    private readonly GroupId _extraGroupId = GroupId.New();
    private readonly UserRole _mainUserRole = UserRolesData.MainUserRole();
    private readonly UserRoleId _extraUserRoleId = UserRoleId.New();

    public AssignmentsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser(_mainUserRole.Id);
        _mainCourse = CoursesData.MainCourse(_mainUser.Id, _mainGroup.Id);
        _mainAssignment = AssignmentsData.MainAssignment(_mainCourse.Id);
    }

    [Fact]
    public async Task ShouldCreateAssignment()
    {
        // Arrange
        var dueDate = DateTime.UtcNow.AddDays(7);
        var title = "Assignment";
        var description = "Assignment description";
        var maxScore = 100m;
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainCourse.Id.ToString()), "courseId" },
            { new StringContent(title), "title" },
            { new StringContent(description), "description" },
            { new StringContent(dueDate.ToLocalTime().ToString()), "dueDate" },
            { new StringContent(maxScore.ToString()), "maxScore" },
        };

        // Act
        var response = await Client.PostAsync("/assignments/create", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<AssignmentDto>();
        responseModel!.Should().NotBeNull();

        // Check if the assignment was created in the database
        var assignmentId = new AssignmentId(responseModel.Id);
        var assignmentFromDatabase = await Context.Assignments.FirstOrDefaultAsync(x => x.Id == assignmentId);
        assignmentFromDatabase.Should().NotBeNull();

        assignmentFromDatabase!.CourseId.Should().Be(_mainCourse.Id);
        assignmentFromDatabase.Title.Should().Be(title);
        assignmentFromDatabase.Description.Should().Be(description);
        assignmentFromDatabase.DueDate.ToString().Should().Be(dueDate.ToLocalTime().ToUniversalTime().ToString());
        assignmentFromDatabase.MaxScore.Should().Be(maxScore);
    }

    [Fact]
    public async Task ShouldNotCreateAssignment_Because_CourseNotFound()
    {
        // Arrange
        var invalidCourseId = Guid.NewGuid().ToString();
        var title = "Assignment";
        var description = "Assignment description";
        var dueDate = DateTime.UtcNow.AddDays(7);
        var maxScore = 100m;
        var formData = new MultipartFormDataContent
        {
            { new StringContent(invalidCourseId), "courseId" },
            { new StringContent(title), "title" },
            { new StringContent(description), "description" },
            { new StringContent(dueDate.ToLocalTime().ToString()), "dueDate" },
            { new StringContent(maxScore.ToString()), "maxScore" },
        };

        // Act
        var response = await Client.PostAsync("/assignments/create", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldUpdateAssignment()
    {
        // Arrange
        var dueDate = DateTime.UtcNow.AddDays(7);
        var newTitle = "Updated assignment";
        var newDescription = "Updated assignment description";
        var newMaxScore = 50m;
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainAssignment.Id.ToString()), "id" },
            { new StringContent(newTitle), "title" },
            { new StringContent(newDescription), "description" },
            { new StringContent(dueDate.ToLocalTime().ToString()), "dueDate" },
            { new StringContent(newMaxScore.ToString()), "maxScore" },
        };

        // Act
        var response = await Client.PutAsync("/assignments/update", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<AssignmentDto>();
        responseModel!.Should().NotBeNull();

        // Check if the assignment was updated in the database
        var assignmentId = new AssignmentId(responseModel.Id);
        var assignmentFromDatabase = await Context.Assignments.FirstOrDefaultAsync(x => x.Id == assignmentId);
        assignmentFromDatabase.Should().NotBeNull();

        assignmentFromDatabase!.CourseId.Should().Be(_mainCourse.Id);
        assignmentFromDatabase.Title.Should().Be(newTitle);
        assignmentFromDatabase.Description.Should().Be(newDescription);
        assignmentFromDatabase.DueDate.ToString().Should().Be(dueDate.ToLocalTime().ToUniversalTime().ToString());
        assignmentFromDatabase.MaxScore.Should().Be(newMaxScore);
    }

    [Fact]
    public async Task ShouldUpdateCourseForAssignment()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainAssignment.Id.ToString()), "id" },
            { new StringContent(_extraCourseId.ToString()), "courseId" },
        };

        // Act
        var response = await Client.PutAsync("/assignments/update-course", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<AssignmentDto>();
        responseModel!.Should().NotBeNull();
        responseModel!.CourseId.Should().Be(_extraCourseId.Value);

        // Check if the assignment was updated in the database
        var assignmentId = new AssignmentId(responseModel.Id);
        var assignmentFromDatabase = await Context.Assignments.FirstOrDefaultAsync(x => x.Id == assignmentId);
        assignmentFromDatabase.Should().NotBeNull();
        assignmentFromDatabase!.CourseId.Should().Be(_extraCourseId);
        assignmentFromDatabase.Title.Should().Be(_mainAssignment.Title);
        assignmentFromDatabase.Description.Should().Be(_mainAssignment.Description);
        assignmentFromDatabase.DueDate.ToString().Should().Be(_mainAssignment.DueDate.ToString());
        assignmentFromDatabase.MaxScore.Should().Be(_mainAssignment.MaxScore);
    }


    [Fact]
    public async Task ShouldNotUpdateCourseForAssignment_Because_CourseNotFound()
    {
        // Arrange
        var invalidCourseId = Guid.NewGuid().ToString();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainAssignment.Id.ToString()), "id" },
            { new StringContent(invalidCourseId), "courseId" },
        };

        // Act
        var response = await Client.PutAsync("/assignments/update-course", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateCourseForAssignment_Because_AssignmentNotFound()
    {
        // Arrange
        var invalidAssignmentId = Guid.NewGuid().ToString();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(invalidAssignmentId), "id" },
            { new StringContent(_extraCourseId.ToString()), "courseId" },
        };

        // Act
        var response = await Client.PutAsync("/assignments/update-course", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateAssignment_Because_AssignmentNotFound()
    {
        // Arrange
        var invalidAssignmentId = Guid.NewGuid().ToString();
        var title = "Assignment";
        var description = "Assignment description";
        var dueDate = DateTime.UtcNow.AddDays(7);
        var maxScore = 100m;
        var formData = new MultipartFormDataContent
        {
            { new StringContent(invalidAssignmentId), "id" },
            { new StringContent(title), "title" },
            { new StringContent(description), "description" },
            { new StringContent(dueDate.ToLocalTime().ToString()), "dueDate" },
            { new StringContent(maxScore.ToString()), "maxScore" },
        };

        // Act
        var response = await Client.PutAsync("/assignments/update", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteAssignment()
    {
        // Arrange & Act
        var response = await Client.DeleteAsync($"/assignments/delete?id={_mainAssignment.Id}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Check if the assignment was deleted from the database
        var assignmentId = new AssignmentId(_mainAssignment.Id.Value);
        var assignmentFromDatabase = await Context.Assignments.FirstOrDefaultAsync(x => x.Id == assignmentId);
        assignmentFromDatabase.Should().BeNull();
    }


    [Fact]
    public async Task ShouldNotDeleteAssignment_Because_AssignmentNotFound()
    {
        // Arrange
        var invalidAssignmentId = Guid.NewGuid().ToString();

        // Act
        var response = await Client.DeleteAsync($"/assignments/delete?id={invalidAssignmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
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

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Assignments.RemoveRange(Context.Assignments);
        Context.Courses.RemoveRange(Context.Courses);
        Context.Users.RemoveRange(Context.Users);
        Context.UserRoles.RemoveRange(Context.UserRoles);
        Context.Groups.RemoveRange(Context.Groups);

        await SaveChangesAsync();
    }
}
