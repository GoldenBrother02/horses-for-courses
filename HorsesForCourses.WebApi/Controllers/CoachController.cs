using System;
using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;

namespace MyLittleWebApi.Exercise.Controllers;

[ApiController]
[Route("coaches")]
public class CoachController : ControllerBase
{
    private readonly InMemoryCoachRepository _repository;
    private readonly CoachMapper _coachMap;
    private readonly CourseMapper _courseMap;

    public CoachController(InMemoryCoachRepository repository, CoachMapper coachMap, CourseMapper coursemap)
    {
        _repository = repository;
        _coachMap = coachMap;
        _courseMap = coursemap;
    }

    [HttpGet("{id}")]
    public ActionResult<CoachDTO> GetById(int id)
    {
        var coach = _repository.GetById(id);
        return coach is null ? NotFound() : Ok(new CoachDTO(coach.Id, coach.Name, coach.Email.ToString(), coach.competencies.ToList(), _courseMap.ListToIdName(coach.CourseList)));
    }

    [HttpPost]
    public ActionResult RegisterCoach([FromBody] PostCoach post)
    {
        var coach = _coachMap.Map(post, _repository.NewId());
        _repository.Add(coach);
        return Ok(coach.Id);
    }

    [HttpPost("{id}/skills")]
    public ActionResult OverwriteSkillset(int id, [FromBody] List<string> NewSkills)
    {
        var coach = _repository.GetById(id);
        if (coach is null) { return NotFound(); }

        coach.OverwriteCompetenties(NewSkills);

        return Ok();
    }

    [HttpGet]
    public ActionResult GetAllCoaches()
    {
        return Ok(_repository.GetAll());
    }
}