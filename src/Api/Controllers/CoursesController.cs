using Api.Dtos.Courses;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Courses.Exceptions;
using Domain.Courses;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CoursesController(ICourseQueries courseQueries) : ControllerBase
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
    }
}
