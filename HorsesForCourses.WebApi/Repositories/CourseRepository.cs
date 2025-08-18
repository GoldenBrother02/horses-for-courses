using System;
using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi;

public class CourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }

    public int GetNextId(AppDbContext context)
    {
        return context.Courses.Any() ? context.Courses.Max(c => c.Id) : 0;
    }

    public async Task<Course> GetCourseById(int id)
    {
        var course = await _context.Courses.Include(c => c.Planning).FirstOrDefaultAsync(e => e.Id == id);
        return course!;
    }

    public async Task<Course> CreateCourse(PostCourse post)
    {
        var result = new Course(GetNextId(_context) + 1, post.Name, post.Start, post.End);
        _context.Courses.Add(result);
        await _context.SaveChangesAsync();
        return result;
    }

    public async Task OverwriteRequirements(Course course, List<string> NewSkills)
    {
        course.OverwriteRequirements(NewSkills);
        await _context.SaveChangesAsync();
    }

    public async Task OverwriteCourseMoments(Course course, List<TimeSlotDTO> NewMoments)
    {
        var list = NewMoments.Select(m => new TimeSlot(m.Day, m.Start, m.End)).ToList();

        course.OverwriteMoments(list);
        await _context.SaveChangesAsync();
    }

    public async Task<PagedResult<CourseDTO>> GetAllCourses(int page, int size, CancellationToken ct)
    {
        var request = new PageRequest(page, size);
        var list = await _context.Courses
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new CourseDTO(
                c.CourseName,
                c.StartDate,
                c.EndDate,
                c.Id,
                c.RequiredCompetencies,
                c.Planning.Select(slot => new TimeSlotDTO(slot.Day, slot.Start, slot.End)).ToList(),
                c.coach == null ? null! : new IdNameCoach(c.coach!.Id, c.coach.Name, c.coach.Email.ToString())))
            .ToPagedResultAsync(request, ct);
        return list;
    }

    public async Task ConfirmCourse(Course course)
    {
        course.ConfirmCourse();
        await _context.SaveChangesAsync();
    }

    public async Task AddCoach(Course course, Coach coach)
    {
        course.AddCoach(coach);
        await _context.SaveChangesAsync();
    }
}