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

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int size = 10, CancellationToken ct = default)
    {
        var list = await _service.GetAllCourses(page, size, ct);
        if (list == null)
            return NotFound();
        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> EditMenu(int id)
    {

        var course = await _service.GetCourseById(id);
        if (course == null)
            return NotFound();

        var model = new EditCourse
        {
            CourseId = course.Id,
            Skills = course.RequiredCompetencies.ToList(),
            Moments = _service.TimeSlotListToTimeSlotDTOList(course.Planning.ToList())
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSkills(EditCourse model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success = await _service.OverwriteRequirements(model.CourseId, model.Skills);
        if (!success)
            return NotFound();

        return RedirectToAction(nameof(GetById), new { id = model.CourseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMoments(EditCourse model)
    {
        if (!ModelState.IsValid)
            return View("EditMenu", model);

        var success = await _service.OverwriteCourseMoments(model.CourseId, model.Moments);
        if (!success)
            return NotFound();

        return RedirectToAction(nameof(GetById), new { id = model.CourseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(int id)
    {
        var success = await _service.ConfirmCourse(id);

        if (!success)
            return NotFound();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> AddCoachMenu(int id)
    {

        var course = await _service.GetCourseById(id);
        if (course == null)
            return NotFound();

        var model = new AddCoachToCourse
        {
            CourseId = course.Id,
            CoachId = -1 //number needed to initialise, but one that guarantees an error if not changed
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCoach(AddCoachToCourse model)
    {
        var success = await _service.AddCoach(model.CourseId, model.CoachId);

        if (!success)
            return NotFound();

        return RedirectToAction(nameof(Index));
    }
}