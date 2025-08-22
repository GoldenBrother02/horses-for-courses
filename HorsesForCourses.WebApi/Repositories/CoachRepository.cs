using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi;

public class CoachRepository
{
    private readonly AppDbContext _context;

    public CoachRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Coach> GetCoachById(int id)
    {
        var coach = await _context.Coaches.Include(c => c.CourseList).FirstOrDefaultAsync(e => e.Id == id);
        return coach!;
    }

    public async Task<Coach> CreateCoach(Coach post)
    {
        await _context.Coaches.AddAsync(post);
        return post;
    }

    public async Task<PagedResult<CoachDTO>> GetAllCoaches(int page, int size, CancellationToken ct)
    {
        var request = new PageRequest(page, size);
        var result = await _context.Coaches
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new CoachDTO(
                c.Id,
                c.Name,
                c.Email.ToString(),
                c.competencies.ToList(),
                c.CourseList.Select(course => new IdNameCourse(course.Id, course.CourseName)).ToList()))
            .ToPagedResultAsync(request, ct);
        return result;
    }
}