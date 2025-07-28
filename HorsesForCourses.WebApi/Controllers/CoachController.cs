using System;
using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;

namespace MyLittleWebApi.Exercise.Controllers;

[ApiController]
[Route("coaches")]
public class CoachController : ControllerBase
{
    private readonly InMemoryCoachRepository _repository;

    public CoachController(InMemoryCoachRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}")]
    public ActionResult<CoachDTO> GetById(Guid id)
    {
        var coach = _repository.GetById(id);
        return coach is null ? NotFound() : Ok(new CoachDTO(coach.Name, coach.Email.Value.ToString(), coach.competencies.ToList()));
    }

    [HttpPost]
    public ActionResult RegisterCoach([FromBody] CoachDTO data)
    {
        var coach = new Coach(data.Name!, data.Email!);
        _repository.Add(coach);
        return Ok(coach.Id);
    }

    [HttpPost("{id}/skills")]
    public ActionResult OverwriteSkillset(Guid id, [FromBody] List<string> NewSkills)
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