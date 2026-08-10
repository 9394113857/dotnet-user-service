using DotNetUserService.Models;

namespace DotNetUserService.Services;

public interface IUserService
{
    Task<List<User>> GetAllAsync();

    Task<User?> GetByIdAsync(int id);

    Task<User> CreateAsync(User user);

    Task<User?> UpdateAsync(int id, User user);

    Task<bool> DeleteAsync(int id);
}