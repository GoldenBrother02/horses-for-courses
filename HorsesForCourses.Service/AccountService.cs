using HorsesForCourses.Core;

namespace HorsesForCourses.Service;

public interface IAccountService
{
    Task<AppUser> CreateUser(AppUser user);
}

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repo;

    public AccountService(IAccountRepository repo)
    {
        _repo = repo;
    }
    public async Task<AppUser> CreateUser(AppUser user)
    {
        var created = await _repo.CreateUser(user);
        await _repo.Save();
        return created;
    }
}