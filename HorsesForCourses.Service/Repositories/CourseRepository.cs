using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Service
{
    public interface ICourseRepository
    {
        Task<Course?> GetCourseById(int id);
        Task<Course> CreateCourse(Course course);
        Task Save();
        Task<PagedResult<CourseDTO>> GetAllCourses(int page, int size, CancellationToken ct = default);
    }
}

namespace HorsesForCourses.Service
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Course?> GetCourseById(int id)
        {
            return await _context.Courses.Include(c => c.Planning)
                                         .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Course> CreateCourse(Course course)
        {
            await _context.Courses.AddAsync(course);
            return course;
        }

        public async Task<PagedResult<CourseDTO>> GetAllCourses(int page, int size, CancellationToken ct = default)
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
                    c.Status,
                    c.RequiredCompetencies,
                    c.Planning.Select(slot => new TimeSlotDTO(slot.Day, slot.Start, slot.End)).ToList(),
                    c.coach == null ? null! : new IdNameCoach(c.coach!.Id, c.coach.Name, c.coach.Email.ToString())))
                .ToPagedResultAsync(request, ct);

            return list;
        }
    }
}
