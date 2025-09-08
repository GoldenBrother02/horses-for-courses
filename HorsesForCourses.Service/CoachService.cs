using HorsesForCourses.Core;

namespace HorsesForCourses.Service;

public interface ICoachService
{
    Task<Coach?> GetCoachById(int id);
    Task<Coach> CreateCoach(Coach coach);
    Task<bool> OverwriteCoachSkillset(int id, List<string> NewSkills);
    Task<PagedResult<CoachDTO>> GetAllCoaches(int page = 1, int size = 10, CancellationToken ct = default);
}

public class CoachService : ICoachService
{
    private readonly ICoachRepository _repo;

    public CoachService(ICoachRepository repo)
    {
        _repo = repo;
    }

    public async Task<Coach?> GetCoachById(int id) => await _repo.GetCoachById(id);

    public async Task<Coach> CreateCoach(Coach coach)
    {
        var created = await _repo.CreateCoach(coach);
        await _repo.Save();
        return created;
    }

    public async Task<bool> OverwriteCoachSkillset(int id, List<string> NewSkills)
    {
        var coach = await _repo.GetCoachById(id);
        if (coach is null) return false;

        coach.OverwriteCompetenties(NewSkills);
        await _repo.Save();
        return true;
    }

    public async Task<PagedResult<CoachDTO>> GetAllCoaches(int page = 1, int size = 10, CancellationToken ct = default)
    {
        return await _repo.GetAllCoaches(page, size, ct);
    }
}

