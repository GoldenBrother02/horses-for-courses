using HorsesForCourses.Core;
using HorsesForCourses.Service;

public interface ICourseService
{
    Task<Course> GetCourseById(int id);
    Task<Course> CreateCourse(Course course);
    Task<bool> OverwriteRequirements(int id, List<string> NewSkills);
    Task<PagedResult<CourseDTO>> GetAllCourses(int page = 1, int size = 10, CancellationToken ct = default);
    Task<bool> OverwriteCourseMoments(int id, List<TimeSlotDTO> NewMoments);
    Task<bool> ConfirmCourse(int id);
    Task<bool> AddCoach(int CourseId, int CoachId);
}

public class CourseService : ICourseService
{

    private readonly CourseRepository _repo;
    private readonly CoachRepository _coachRepo;

    public CourseService(CourseRepository repo, CoachRepository coachrepo)
    {
        _repo = repo;
        _coachRepo = coachrepo;
    }
    public async Task<Course> GetCourseById(int id)
    {
        var course = await _repo.GetCourseById(id);
        return course;
    }

    public async Task<Course> CreateCourse(Course course)
    {
        var Created = await _repo.CreateCourse(course);
        await _repo.Save();
        return Created;
    }

    public async Task<bool> OverwriteRequirements(int id, List<string> NewSkills)
    {
        var course = await _repo.GetCourseById(id);
        if (course is null) { return false; }

        course.OverwriteRequirements(NewSkills);
        await _repo.Save();

        return true;
    }

    public async Task<PagedResult<CourseDTO>> GetAllCourses(int page = 1, int size = 10, CancellationToken ct = default)
    {
        var result = await _repo.GetAllCourses(page, size, ct);
        return result;
    }

    public async Task<bool> OverwriteCourseMoments(int id, List<TimeSlotDTO> NewMoments)
    {
        var course = await _repo.GetCourseById(id);
        if (course is null) { return false; }
        var list = NewMoments.Select(m => TimeSlot.From(m.Day, m.Start, m.End)).ToList();

        course.OverwriteMoments(list);
        await _repo.Save();

        return true;
    }

    public async Task<bool> ConfirmCourse(int id)
    {
        var course = await _repo.GetCourseById(id);
        if (course is null) { return false; }

        course.ConfirmCourse();
        await _repo.Save();

        return true;
    }

    public async Task<bool> AddCoach(int CourseId, int CoachId)
    {
        var course = await _repo.GetCourseById(CourseId);
        if (course is null) { return false; }

        var coach = await _coachRepo.GetCoachById(CoachId);
        if (coach is null) { return false; }

        course.AddCoach(coach);
        await _repo.Save();

        return true;
    }
}