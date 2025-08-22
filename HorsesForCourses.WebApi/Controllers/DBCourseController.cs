using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.WebApi;

[ApiController]
[Route("api/Courses")]
public class DBCourseController : ControllerBase
{
    private readonly CourseRepository _repo;
    private readonly CoachRepository _coachRepo;
    public DBCourseController(CourseRepository repository, CoachRepository coachrepository)
    {
        _repo = repository;
        _coachRepo = coachrepository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Course>> GetCourseById(int id)
    {
        var course = await _repo.GetCourseById(id);
        return course is null ? NotFound() : Ok(course);
    }

    [HttpPost]
    public async Task<ActionResult<Course>> CreateCourse([FromBody] PostCourse post)
    {
        var result = new Course(post.Name, post.Start, post.End);

        await _repo.CreateCourse(result);
        await _repo.Save();

        return CreatedAtAction(nameof(GetCourseById), new { id = result.Id }, result);
    }

    [HttpPost("{id}/Competencies")]
    public async Task<ActionResult> OverwriteRequirements(int id, [FromBody] List<string> NewSkills)
    {
        var course = await _repo.GetCourseById(id);
        if (course is null) { return NotFound(); }

        course.OverwriteRequirements(NewSkills);
        await _repo.Save();

        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CourseDTO>>> GetAllCourses(
    [FromQuery] int page = 1,
    [FromQuery] int size = 10,
    CancellationToken ct = default)
    {
        var list = await _repo.GetAllCourses(page, size, ct);
        return Ok(list);
    }

    [HttpPost("{id}/timeslots")]
    public async Task<ActionResult> OverwriteCourseMoments(int id, [FromBody] List<TimeSlotDTO> NewMoments)
    {
        var course = await _repo.GetCourseById(id);
        if (course is null) { return NotFound(); }

        var list = NewMoments.Select(m => new TimeSlot(m.Day, m.Start, m.End)).ToList();

        course.OverwriteMoments(list);
        await _repo.Save();

        return Ok();
    }

    [HttpPost("{id}/confirm")]
    public async Task<ActionResult> ConfirmCourse(int id)
    {
        var course = await _repo.GetCourseById(id);
        if (course is null) { return NotFound(); }

        course.ConfirmCourse();
        await _repo.Save();

        return Ok();
    }

    [HttpPost("{CourseId}/assign-coach")]
    public async Task<ActionResult> AddCoach(int CourseId, int CoachId)
    {
        var course = await _repo.GetCourseById(CourseId);
        if (course is null) { return NotFound(); }

        var coach = await _coachRepo.GetCoachById(CoachId);
        if (coach is null) { return NotFound(); }

        course.AddCoach(coach);
        await _repo.Save();

        return Ok();
    }
}