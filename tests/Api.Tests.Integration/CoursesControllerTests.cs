using System.Net;
using Api.Dtos.Courses;
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

public class CoursesControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Course _mainCourse;
    private readonly CourseId _extraCourseId = CourseId.New();
    private readonly User _mainUser;
    private readonly UserId _extraUserId = UserId.New();
    private readonly Group _mainGroup;
    private readonly GroupId _extraGroupId = GroupId.New();
    private readonly UserRole _mainUserRole = UserRolesData.MainUserRole();
    private readonly UserRoleId _extraUserRoleId = UserRoleId.New();

    public CoursesControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainGroup = GroupsData.MainGroup();
        _mainUserRole = UserRolesData.MainUserRole();
        _mainUser = UsersData.MainUser(_mainUserRole.Id);
        _mainCourse = CoursesData.MainCourse(_mainUser.Id, _mainGroup.Id);
    }

    [Fact]
    public async Task ShouldCreateCourse()
    {
        // Arrange
        var name = "Test course";
        var description = "Test course description";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(name), "name" },
            { new StringContent(description), "description" },
            { new StringContent(_mainUser.Id.ToString()), "userId" },
            { new StringContent(_mainGroup.Id.ToString()), "groupId" },
        };

        // Act
        var response = await Client.PostAsync("/courses/create", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseModel = await response.ToResponseModel<CourseDto>();

        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(name);
        responseModel.Description.Should().Be(description);

        // Check if the course was created in the database
        var courseId = new CourseId(responseModel.Id);
        var courseFromDatabase = await Context.Courses.FirstOrDefaultAsync(x => x.Id == courseId);

        courseFromDatabase.Should().NotBeNull();
        courseFromDatabase!.Name.Should().Be(name);
        courseFromDatabase.Description.Should().Be(description);
        courseFromDatabase.UserId.Should().Be(_mainUser.Id);
        courseFromDatabase.GroupId.Should().Be(_mainGroup.Id);
    }


    [Fact]
    public async Task ShouldNotCreateCourse_Because_GroupNotFound()
    {
        // Arrange
        var name = "Test course";
        var description = "Test course description";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(name), "name" },
            { new StringContent(description), "description" },
            { new StringContent(Guid.NewGuid().ToString()), "userId" },
            { new StringContent(_mainGroup.Id.ToString()), "groupId" },
        };

        // Act
        var response = await Client.PostAsync("/courses/create", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotCreateCourse_Because_UserNotFound()
    {
        // Arrange
        var name = "Test course";
        var description = "Test course description";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(name), "name" },
            { new StringContent(description), "description" },
            { new StringContent(_mainUser.Id.ToString()), "userId" },
            { new StringContent(Guid.NewGuid().ToString()), "groupId" },
        };

        // Act
        var response = await Client.PostAsync("/courses/create", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldUpdateCourse()
    {
        // Arrange
        var name = "New course";
        var description = "New course description";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainCourse.Id.ToString()), "id" },
            { new StringContent(name), "name" },
            { new StringContent(description), "description" },
        };

        // Act
        var response = await Client.PutAsync("/courses/update", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseModel = await response.ToResponseModel<CourseDto>();

        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(name);
        responseModel.Description.Should().Be(description);

        // Check if the course was updated in the database
        var courseId = new CourseId(responseModel.Id);
        var courseFromDatabase = await Context.Courses.FirstOrDefaultAsync(x => x.Id == courseId);

        courseFromDatabase.Should().NotBeNull();
        courseFromDatabase!.Name.Should().Be(name);
        courseFromDatabase.Description.Should().Be(description);
    }


    [Fact]
    public async Task ShouldNotUpdateCourse_Because_CourseNotFound()
    {
        // Arrange
        var invalidCourseId = Guid.NewGuid().ToString();
        var name = "New course";
        var description = "New course description";
        var formData = new MultipartFormDataContent
        {
            { new StringContent(invalidCourseId), "id" },
            { new StringContent(name), "name" },
            { new StringContent(description), "description" },
        };

        // Act
        var response = await Client.PutAsync("/courses/update", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldUpdateGroupForCourse()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainCourse.Id.ToString()), "id" },
            { new StringContent(_extraGroupId.ToString()), "groupId" },
        };

        // Act
        var response = await Client.PutAsync("/courses/update-group", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseModel = await response.ToResponseModel<CourseDto>();
        responseModel.GroupId.Should().Be(_extraGroupId.ToString());
        responseModel.Should().NotBeNull();

        // Check if the course was updated in the database
        var courseId = new CourseId(responseModel.Id);
        var courseFromDatabase = await Context.Courses.FirstOrDefaultAsync(x => x.Id == courseId);
        courseFromDatabase.Should().NotBeNull();
        courseFromDatabase!.GroupId.Should().Be(_extraGroupId);
    }

    [Fact]
    public async Task ShouldNotUpdateGroupForCourse_Because_GroupNotFound()
    {
        // Arrange
        var invalidGroupId = Guid.NewGuid().ToString();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainCourse.Id.ToString()), "id" },
            { new StringContent(invalidGroupId), "groupId" },
        };

        // Act
        var response = await Client.PutAsync("/courses/update-group", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateGroupForCourse_Because_CourseNotFound()
    {
        // Arrange
        var invalidCourseId = Guid.NewGuid().ToString();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(invalidCourseId), "id" },
            { new StringContent(_extraGroupId.ToString()), "groupId" },
        };

        // Act
        var response = await Client.PutAsync("/courses/update-group", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldUpdateUserForCourse()
    {
        // Arrange
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainCourse.Id.ToString()), "id" },
            { new StringContent(_extraUserId.ToString()), "userId" },
        };

        // Act
        var response = await Client.PutAsync("/courses/update-user", formData);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseModel = await response.ToResponseModel<CourseDto>();
        responseModel.UserId.Should().Be(_extraUserId.ToString());
        responseModel.Should().NotBeNull();

        // Check if the course was updated in the database
        var courseId = new CourseId(responseModel.Id);
        var courseFromDatabase = await Context.Courses.FirstOrDefaultAsync(x => x.Id == courseId);
        courseFromDatabase.Should().NotBeNull();
        courseFromDatabase!.UserId.Should().Be(_extraUserId);
    }


    [Fact]
    public async Task ShouldNotUpdateUserForCourse_Because_UserNotFound()
    {
        // Arrange
        var invalidUserId = Guid.NewGuid().ToString();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(_mainCourse.Id.ToString()), "id" },
            { new StringContent(invalidUserId), "userId" },
        };

        // Act
        var response = await Client.PutAsync("/courses/update-user", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateUserForCourse_Because_CourseNotFound()
    {
        // Arrange
        var invalidCourseId = Guid.NewGuid().ToString();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(invalidCourseId), "id" },
            { new StringContent(_extraUserId.ToString()), "userId" },
        };

        // Act
        var response = await Client.PutAsync("/courses/update-user", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteCourse()
    {
        // Arrange & Act
        var response = await Client.DeleteAsync($"/courses/delete?id={_mainCourse.Id}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseModel = await response.ToResponseModel<CourseDto>();

        responseModel.Should().NotBeNull();
        responseModel!.Name.Should().Be(_mainCourse.Name);

        // Check if the course was deleted from the database
        var courseId = new CourseId(responseModel.Id);
        var courseFromDatabase = await Context.Courses.FirstOrDefaultAsync(x => x.Id == courseId);
        courseFromDatabase.Should().BeNull();
    }


    [Fact]
    public async Task ShouldNotDeleteCourse_Because_CourseNotFound()
    {
        // Arrange
        var invalidCourseId = Guid.NewGuid().ToString();

        // Act
        var response = await Client.DeleteAsync($"/courses/delete?id={invalidCourseId}");

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

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Courses.RemoveRange(Context.Courses);
        Context.Users.RemoveRange(Context.Users);
        Context.UserRoles.RemoveRange(Context.UserRoles);
        Context.Groups.RemoveRange(Context.Groups);

        await SaveChangesAsync();
    }
}
