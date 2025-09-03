using HorsesForCourses.Core;
using HorsesForCourses.Service;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.MVC;

public class CoursesController : Controller
{
    private readonly ICourseService _service;
    public CoursesController(ICourseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _service.GetCourseById(id);
        if (course == null)
            return NotFound();

        return View(course);
    }

    [HttpGet]
    public IActionResult CreateMenu()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCourse(PostCourse post)
    {
        if (!ModelState.IsValid)
            return View("CreateMenu", post);

        var result = new Course(post.Name, post.Start, post.End);
        await _service.CreateCourse(result);

        return RedirectToAction(nameof(GetById), new { id = result.Id });
    }

    [HttpPost("{id}/Competencies")]
    public async Task<ActionResult> OverwriteRequirements(int id, [FromBody] List<string> NewSkills)
    {
        var success = await _service.OverwriteRequirements(id, NewSkills);

        if (success) { return Ok(); }
        return NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int size = 10, CancellationToken ct = default)
    {
        var list = await _service.GetAllCourses(page, size, ct);
        return View(list);
    }

    [HttpPost("{id}/timeslots")]
    public async Task<ActionResult> OverwriteCourseMoments(int id, [FromBody] List<TimeSlotDTO> NewMoments)
    {
        var success = await _service.OverwriteCourseMoments(id, NewMoments);

        if (success) { return Ok(); }
        return NotFound();
    }

    [HttpPost("{id}/confirm")]
    public async Task<ActionResult> ConfirmCourse(int id)
    {
        var success = await _service.ConfirmCourse(id);

        if (success) { return Ok(); }
        return NotFound();
    }

    [HttpPost("{CourseId}/assign-coach")]
    public async Task<ActionResult> AddCoach(int CourseId, int CoachId)
    {
        var success = await _service.AddCoach(CourseId, CoachId);

        if (success) { return Ok(); }
        return NotFound();
    }
}