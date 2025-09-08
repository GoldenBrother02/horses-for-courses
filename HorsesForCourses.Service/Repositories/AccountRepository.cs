using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Service;

public interface IAccountRepository
{
    Task<AppUser> CreateUser(AppUser user);
    Task Save();
}

public class AccountRepository : IAccountRepository
{

    private readonly AppDbContext _context;

    public AccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<AppUser> CreateUser(AppUser user)
    {
        await _context.Users.AddAsync(user);
        return user;
    }
}
