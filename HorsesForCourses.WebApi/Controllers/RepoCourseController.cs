using System;
using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.WebApi;

[ApiController]
[Route("courses")]
public class CourseController : ControllerBase
{
    private readonly InMemoryCourseRepository _repository;
    private readonly InMemoryCoachRepository _coaches;
    private readonly TimeSlotMapper _timeSlotDTOMap;
    private readonly CourseMapper _courseMap;
    private readonly CoachMapper _coachMap;

    public CourseController(InMemoryCourseRepository repository, InMemoryCoachRepository coaches, TimeSlotMapper timeSlotMap, CourseMapper courseMap, CoachMapper coachMap)
    {
        _repository = repository;
        _coaches = coaches;
        _timeSlotDTOMap = timeSlotMap;
        _courseMap = courseMap;
        _coachMap = coachMap;
    }

    [HttpGet("{id}")] //
    public ActionResult<CourseDTO> GetById(int id)
    {
        var course = _repository.GetById(id);
        return course is null ? NotFound() : Ok(new CourseDTO(course.CourseName, course.StartDate, course.EndDate, course.Id, course.RequiredCompetencies, _timeSlotDTOMap.Revert(course.Planning.ToList()), _coachMap.CoachToIdName(course.coach!)));
    }

    [HttpPost] //
    public ActionResult RegisterCourse([FromBody] PostCourse post)
    {
        var course = _courseMap.PostToCourse(post, _repository.NewId());
        _repository.Add(course);
        return Ok(course.Id);
    }

    [HttpPost("{id}/skills")] //
    public ActionResult OverwriteRequirements(int id, [FromBody] List<string> NewSkills)
    {
        var course = _repository.GetById(id);
        if (course is null) { return NotFound(); }

        course.OverwriteRequirements(NewSkills);
        return Ok();
    }

    [HttpPost("{id}/timeslots")] //
    public ActionResult OverwriteCourseMoments(int id, [FromBody] List<TimeSlotDTO> NewMoments)
    {
        var course = _repository.GetById(id);
        if (course is null) { return NotFound(); }

        var list = _timeSlotDTOMap.Map(NewMoments);

        course.OverwriteMoments(list);

        return Ok();
    }

    [HttpPost("{id}/confirm")] //
    public ActionResult ConfirmCourse(int id)
    {
        var course = _repository.GetById(id);
        if (course is null) { return NotFound(); }

        course.ConfirmCourse();
        return Ok();
    }

    [HttpPost("{CourseId}/assign-coach")] //
    public ActionResult AddCoach(int CourseId, int CoachId)
    {
        var course = _repository.GetById(CourseId);
        if (course is null) { return NotFound(); }

        var coach = _coaches.GetById(CoachId);
        if (coach is null) { return NotFound(); }

        course.AddCoach(coach);
        return Ok();
    }

    [HttpGet] //
    public ActionResult GetAllCourses()
    {
        return Ok(_repository.GetAll());
    }
}