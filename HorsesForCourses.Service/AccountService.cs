using HorsesForCourses.Core;
using SQLitePCL;

namespace HorsesForCourses.Service;

public interface IAccountService
{
    Task<AppUser> CreateUser(AppUser user);
    Task<AppUser?> GetUser(string email);
    Task Deleteuser(AppUser user);
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
    public async Task<AppUser?> GetUser(string email)
    {
        var user = await _repo.GetUser(email);
        return user;
    }
    public async Task Deleteuser(AppUser user)
    {
        _repo.RemoveUser(user);
        await _repo.Save();
    }
}