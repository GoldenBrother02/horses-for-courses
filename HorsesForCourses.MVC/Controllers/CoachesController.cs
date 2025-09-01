using HorsesForCourses.Core;
using HorsesForCourses.Service;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.MVC;

public class CoachesController : Controller
{
    private readonly ICoachService _service;

    public CoachesController(ICoachService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var coach = await _service.GetCoachById(id);
        if (coach == null)
            return NotFound();

        return View(coach);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PostCoach post)
    {
        if (!ModelState.IsValid)
            return View(post);

        var coach = new Coach(post.Name, post.Email);
        await _service.CreateCoach(coach);

        return RedirectToAction(nameof(Details), new { id = coach.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var coach = await _service.GetCoachById(id);
        if (coach == null)
            return NotFound();

        var model = new EditCoachSkills
        {
            CoachId = coach.Id,
            CurrentSkills = coach.competencies.ToList()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditCoachSkills model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success = await _service.OverwriteCoachSkillset(model.CoachId, model.NewSkills);
        if (!success)
            return NotFound();

        return RedirectToAction(nameof(Details), new { id = model.CoachId });
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int size = 10, CancellationToken ct = default)
    {
        var coaches = await _service.GetAllCoaches(page, size, ct);
        return View(coaches);
    }
}
