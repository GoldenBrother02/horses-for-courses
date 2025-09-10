using HorsesForCourses.Core;
using HorsesForCourses.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.MVC;

[Authorize]
public class CoachesController : Controller
{
    private readonly ICoachService _service;


    public CoachesController(ICoachService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var coach = await _service.GetCoachById(id);
        if (coach == null)
            return NotFound();

        return View(coach);
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult CreateMenu()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateCoach(PostCoach post)
    {
        if (!ModelState.IsValid)
            return View("CreateMenu", post);

        var coach = new Coach(post.Name, post.Email);
        var created = await _service.CreateCoach(coach);

        return RedirectToAction(nameof(GetById), new { id = created.Id });
    }

    [HttpGet]
    [Authorize(Roles = "admin, coach")]
    public async Task<IActionResult> EditMenu(int id)
    {
        var coach = await _service.GetCoachById(id);
        if (coach == null)
            return NotFound();

        if (User.IsInRole("coach") && User.Identity?.Name != coach.Email.Value) { return Forbid(); }

        var model = new EditCoachSkills
        {
            CoachId = coach.Id,
            Skills = coach.competencies.ToList()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin, coach")]
    public async Task<IActionResult> EditSkills(EditCoachSkills model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var coach = await _service.GetCoachById(model.CoachId);
        if (User.IsInRole("coach") && User.Identity?.Name != coach!.Email.Value) { return Forbid(); }

        var success = await _service.OverwriteCoachSkillset(model.CoachId, model.Skills);
        if (!success)
            return NotFound();

        return RedirectToAction(nameof(GetById), new { id = model.CoachId });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index(int page = 1, int size = 5, CancellationToken ct = default)
    {
        var coaches = await _service.GetAllCoaches(page, size, ct);
        return View(coaches);
    }
}
