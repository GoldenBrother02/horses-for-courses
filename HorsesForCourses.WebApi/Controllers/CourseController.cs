using System;
using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;

namespace MyLittleWebApi.Exercise.Controllers;

[ApiController]
[Route("courses")]
public class CourseController : ControllerBase
{
    private readonly InMemoryCourseRepository _repository;
    private readonly InMemoryCoachRepository _coaches;

    public CourseController(InMemoryCourseRepository repository, InMemoryCoachRepository coaches)
    {
        _repository = repository;
        _coaches = coaches;
    }

    [HttpGet("{id}")]
    public ActionResult<CourseDTO> GetById(Guid id)
    {
        var course = _repository.GetById(id);
        return course is null ? NotFound() : Ok(new CourseDTO(course.CourseName, course.StartDate, course.EndDate));
    }

    [HttpPost]
    public ActionResult RegisterCourse([FromBody] CourseDTO data)
    {
        var course = new Course(data.Name!, data.Start, data.End);
        _repository.Add(course);
        return Ok(course.Id);
    }

    [HttpPost("{id}/skills")]
    public ActionResult OverwriteRequirements(Guid id, [FromBody] List<string> NewSkills)
    {
        var course = _repository.GetById(id);
        if (course is null) { return NotFound(); }

        course.OverwriteRequirements(NewSkills);
        return Ok();
    }

    [HttpPost("{id}/timeslots")]
    public ActionResult OverwriteCourseMoments(Guid id, [FromBody] List<TimeSlotDTO> NewMoments)
    {
        var course = _repository.GetById(id);
        if (course is null) { return NotFound(); }

        var list = new List<TimeSlot>();

        foreach (var slot in NewMoments)
        {
            var newslot = new TimeSlot(slot.Day, slot.Start, slot.End);
            list.Add(newslot);
        }

        course.OverwriteMoments(list);

        return Ok();
    }

    [HttpPost("{id}/confirm")]
    public ActionResult ConfirmCourse(Guid id)
    {
        var course = _repository.GetById(id);
        if (course is null) { return NotFound(); }

        course.ConfirmCourse();
        return Ok();
    }

    [HttpPost("{CourseId}/assign-coach")]
    public ActionResult AddCoach(Guid CourseId, Guid CoachId)
    {
        var course = _repository.GetById(CourseId);
        if (course is null) { return NotFound(); }

        var coach = _coaches.GetById(CoachId);
        if (coach is null) { return NotFound(); }

        course.AddCoach(coach);
        return Ok();
    }

    [HttpGet]
    public ActionResult GetAllCourses()
    {
        return Ok(_repository.GetAll());
    }
}