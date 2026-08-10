using DotNetUserService.Data;
using DotNetUserService.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetUserService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _db.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> CreateAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return user;
    }

    public async Task<User?> UpdateAsync(int id, User updatedUser)
    {
        var user = await _db.Users.FindAsync(id);

        if (user == null)
        {
            return null;
        }

        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;

        await _db.SaveChangesAsync();

        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user == null)
        {
            return false;
        }

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return true;
    }
}