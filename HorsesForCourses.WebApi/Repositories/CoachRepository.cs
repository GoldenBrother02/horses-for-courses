using System;
using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi;

public class CoachRepository
{
    private readonly AppDbContext _context;

    public CoachRepository(AppDbContext context)
    {
        _context = context;
    }

    public int GetNextId(AppDbContext context)
    {
        return context.Coaches.Any() ? context.Coaches.Max(c => c.Id) : 0;
    }

    public async Task<Coach> GetCoachById(int id)
    {
        var coach = await _context.Coaches.Include(c => c.CourseList).FirstOrDefaultAsync(e => e.Id == id);
        return coach!;
    }

    public async Task<Coach> CreateCoach(PostCoach post)
    {
        var result = new Coach(GetNextId(_context) + 1, post.Name, post.Email);
        _context.Coaches.Add(result);
        await _context.SaveChangesAsync();
        return result;
    }

    public async Task OverwriteCoachSkillset(Coach coach, List<string> NewSkills)
    {
        coach.OverwriteCompetenties(NewSkills);
        await _context.SaveChangesAsync();
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