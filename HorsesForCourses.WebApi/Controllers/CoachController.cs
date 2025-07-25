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
    public ActionResult<Coach> GetById(Guid id)
    {
        var coach = _repository.GetById(id);
        return coach is null ? NotFound() : Ok(coach);
    }

    [HttpPost]
    public ActionResult RegisterCoach([FromBody] CoachDTO data)
    {
        var coach = new Coach(data.Name!, data.Email!);
        _repository.Add(coach);
        return Ok(coach);
    }

    [HttpPost("{id}/skills")]
    public ActionResult OverwriteSkillset(Guid id, [FromBody] List<string> NewSkills)
    {
        var coach = _repository.GetById(id);
        if (coach is null) { return NotFound(); }

        var currentSkills = coach.competencies.ToList();
        //kan lijst niet editen terwijl je loopt, dus maak copy om over te loopen

        foreach (var skill in currentSkills) { coach.RemoveCompetence(skill); }
        foreach (var skill in NewSkills) { coach.AddCompetence(skill); }

        return Ok();
    }

    [HttpGet]
    public ActionResult GetAllCoaches()
    {
        return Ok(_repository.GetAll());
    }
}