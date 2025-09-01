using HorsesForCourses.Core;
using HorsesForCourses.Service;
using Microsoft.EntityFrameworkCore;

public interface ICoachService
{
    Task<Coach> GetCoachById(int id);
    Task<Coach> CreateCoach(Coach coach);
    Task<bool> OverwriteCoachSkillset(int id, List<string> NewSkills);
    Task<PagedResult<CoachDTO>> GetAllCoaches(int page = 1, int size = 10, CancellationToken ct = default);
}

public class CoachService : ICoachService
{
    private readonly CoachRepository _repo;

    public CoachService(CoachRepository repo)
    {
        _repo = repo;
    }

    public async Task<Coach> GetCoachById(int id)
    {
        var coach = await _repo.GetCoachById(id);
        return coach;
    }

    public async Task<Coach> CreateCoach(Coach coach)
    {
        var Created = await _repo.CreateCoach(coach);
        await _repo.Save();
        return Created;
    }

    public async Task<bool> OverwriteCoachSkillset(int id, List<string> NewSkills)
    {
        var coach = await _repo.GetCoachById(id);
        if (coach is null) { return false; }

        coach.OverwriteCompetenties(NewSkills);
        await _repo.Save();

        return true;
    }

    public async Task<PagedResult<CoachDTO>> GetAllCoaches(int page = 1, int size = 10, CancellationToken ct = default)
    {
        var result = await _repo.GetAllCoaches(page, size, ct);
        return result;
    }
}
