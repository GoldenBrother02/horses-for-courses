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
    public ActionResult<Course> GetById(Guid id)
    {
        var course = _repository.GetById(id);
        return course is null ? NotFound() : Ok(course);
    }

    [HttpPost]
    public ActionResult RegisterCourse([FromBody] CourseDTO data)
    {
        var course = new Course(data.Name!, data.Start, data.End);
        _repository.Add(course);
        return Ok(course);
    }

    [HttpPost("{id}/skills")]
    public ActionResult OverwriteRequirements(Guid id, [FromBody] List<string> NewSkills)
    {
        var course = _repository.GetById(id);
        if (course is null) { return NotFound(); }

        var currentSkills = course.RequiredCompetencies.ToList();
        //kan lijst niet editen terwijl je loopt, dus maak copy om over te loopen

        foreach (var req in currentSkills) { course.RemoveRequirement(req); }
        foreach (var req in NewSkills) { course.AddRequirement(req); }
        return Ok();
    }

    [HttpPost("{id}/timeslots")]
    public ActionResult OverwriteCourseMoments(Guid id, [FromBody] List<TimeSlotDTO> NewMoments)
    {
        var course = _repository.GetById(id);
        if (course is null) { return NotFound(); }

        var currentSlots = course.Planning.ToList();
        //kan lijst niet editen terwijl je loopt, dus maak copy om over te loopen

        foreach (var slot in currentSlots) { course.RemoveCourseMoment(slot); }

        foreach (var slot in NewMoments)
        {
            var newslot = new TimeSlot(slot.Day, slot.Start, slot.End);
            course.AddCourseMoment(newslot);
        }
        //cleared eerst de planning en voegt daarna toe, als de nieuwe overlap hebben en error geven heb je maar een aantal moments
        //van degene die je wou staan in de course, terwijl de geldige vorige lijst al weg is
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
}