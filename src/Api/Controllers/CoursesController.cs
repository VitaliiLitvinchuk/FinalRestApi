using Api.Dtos.Courses;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Courses.Commands;
using Application.Courses.Exceptions;
using Domain.Courses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CoursesController(ICourseQueries courseQueries, ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CourseDto>>> GetAll(CancellationToken cancellationToken)
        {
            var courses = await courseQueries.GetAllAsync(cancellationToken);

            return Ok(courses.Select(CourseDto.FromDomainModel).ToList());
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<CourseDto>> GetById([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                return BadRequest();

            CourseId courseId = new(id);

            var course = await courseQueries.GetByIdAsync(courseId, cancellationToken);

            return course.Match<ActionResult<CourseDto>>(
                course => Ok(CourseDto.FromDomainModel(course)),
                () => new CourseNotFoundException(courseId).ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<CourseDto>> Create([FromForm] CourseCreateDto dto, CancellationToken cancellationToken)
        {
            var input = new CreateCourseCommand
            {
                Name = dto.Name,
                Description = dto.Description,
                UserId = dto.UserId,
                GroupId = dto.GroupId
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<CourseDto>>(
                course => Ok(CourseDto.FromDomainModel(course)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<CourseDto>> Update([FromForm] CourseUpdateDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateCourseCommand
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<CourseDto>>(
                course => Ok(CourseDto.FromDomainModel(course)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<CourseDto>> UpdateGroup([FromForm] CourseUpdateGroupDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateGroupForCourseCommand
            {
                Id = dto.Id,
                GroupId = dto.GroupId
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<CourseDto>>(
                course => Ok(CourseDto.FromDomainModel(course)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<CourseDto>> UpdateUser([FromForm] CourseUpdateUserDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateUserForCourseCommand
            {
                Id = dto.Id,
                UserId = dto.UserId
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<CourseDto>>(
                course => Ok(CourseDto.FromDomainModel(course)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult<CourseDto>> Delete([FromQuery] CourseDeleteDto dto, CancellationToken cancellationToken)
        {
            var input = new DeleteCourseCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<CourseDto>>(
                course => Ok(CourseDto.FromDomainModel(course)),
                e => e.ToObjectResult()
            );
        }
    }
}
