using Core.Entities;
using Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class UsersRepository : IUsersRepository
{
    private readonly AppdbContext _dbContext;

    public UsersRepository(AppdbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Users?> AddUserAsync(Users? user)
    {
        if (user is null)
            return null;

        var check = await GetUserByEmailAsync(user.Email!);

        if (check is not null)
            return null;

        await _dbContext.Users.AddAsync(user);

        var result = await _dbContext.SaveChangesAsync();

        return result > 0 ? user : null;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.UserID == userId);

        if (user is null)
            return false;

        user.Roles.Clear();

        _dbContext.Users.Remove(user);

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<Users>> GetAllUsersAsync()
    {
        return await _dbContext.Users
            .ToListAsync();
    }

    public async Task<Users?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Users?> GetUserByIDAsync(Guid userID)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.UserID == userID);
    }

    public async Task<Users?> UpdateUserAsync(Users user)
    {
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.UserID == user.UserID);

        if (existingUser is null)
            return null;

        existingUser.Email = user.Email;
        existingUser.PasswordHash = user.PasswordHash;
        existingUser.Name = user.Name;
        existingUser.Gender = user.Gender;
        existingUser.RefreshToken = user.RefreshToken;
        existingUser.RefreshTokenExpiryTime = user.RefreshTokenExpiryTime;
        existingUser.GoogleId = user.GoogleId;

        var result = await _dbContext.SaveChangesAsync();

        return result > 0 ? existingUser : null;
    }

    public async Task<bool> AddRoleToUserAsync(Guid userId, Guid roleId)
    {
        var user = await _dbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.UserID == userId);

        if (user is null)
            return false;

        var role = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.RoleId == roleId);

        if (role is null)
            return false;

        if (user.Roles.Any(r => r.RoleId == roleId))
            return false;

        user.Roles.Add(role);

        return await _dbContext.SaveChangesAsync() > 0;
    }
}
