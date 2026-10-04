using Core.Entities;
using Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository;

public class RolesRepository : IRolesRepository
{
    private readonly AppdbContext _dbContext;
    private readonly ILogger<RolesRepository> _logger;

    public RolesRepository(AppdbContext dbContext,ILogger<RolesRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<Roles>> GetAllRolesAsync()
    {
        return await _dbContext.Roles
            .ToListAsync();
    }

    public async Task<IEnumerable<Roles>> GetRolesByUserIdAsync(Guid userId)
    {
        return await _dbContext.Users
            .Where(u => u.UserID == userId)
            .SelectMany(u => u.Roles)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetRolesIdsByNameAsync(IEnumerable<string> roleNames)
    {
        return await _dbContext.Roles
            .Where(r => roleNames.Contains(r.RoleName))
            .Select(r => r.RoleId)
            .ToListAsync();
    }

    public async Task<bool> UpdateRoleAsync(Guid roleId,string newRoleName)
    {
        var role = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.RoleId == roleId);

        if (role is null)
            return false;

        role.RoleName = newRoleName;

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateUsersRolesAsync(Guid userId,IEnumerable<string> roleNames)
    {
        try
        {
            var user = await _dbContext.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user is null)
                return false;

            var roles = await _dbContext.Roles
                .Where(r => roleNames.Contains(r.RoleName))
                .ToListAsync();

            user.Roles.Clear();

            foreach (var role in roles)
            {
                user.Roles.Add(role);
            }

            await _dbContext.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while updating user roles for userId: {UserId}",
                userId);

            return false;
        }
    }

    public async Task<bool> AddRoleAsync(Roles role)
    {
        await _dbContext.Roles.AddAsync(role);

        return await _dbContext.SaveChangesAsync() > 0;
    }
}
