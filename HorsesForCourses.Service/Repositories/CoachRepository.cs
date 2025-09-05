using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Service
{
    public interface ICoachRepository
    {
        Task<Coach?> GetCoachById(int id);
        Task<Coach> CreateCoach(Coach coach);
        Task Save();
        Task<PagedResult<CoachDTO>> GetAllCoaches(int page, int size, CancellationToken ct = default);
    }
}

namespace HorsesForCourses.Service
{
    public class CoachRepository : ICoachRepository
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

        public async Task<Coach?> GetCoachById(int id)
        {
            return await _context.Coaches.Include(c => c.CourseList)
                                         .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Coach> CreateCoach(Coach coach)
        {
            await _context.Coaches.AddAsync(coach);
            return coach;
        }

        public async Task<PagedResult<CoachDTO>> GetAllCoaches(int page, int size, CancellationToken ct = default)
        {
            var request = new PageRequest(page, size);
            var result = await _context.Coaches
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Select(c => new CoachDTO(
                    c.Id,
                    c.Name,
                    c.Email.Value.ToString(),
                    c.competencies.ToList(),
                    c.CourseList.Select(course => new IdNameCourse(course.Id, course.CourseName)).ToList()))
                .ToPagedResultAsync(request, ct);

            return result;
        }
    }
}
