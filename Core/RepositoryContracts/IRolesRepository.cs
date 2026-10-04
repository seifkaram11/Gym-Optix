using Core.Entities;

namespace Core.RepositoryContracts;

public interface IRolesRepository
{
    Task<IEnumerable<Roles>> GetAllRolesAsync();
    Task<IEnumerable<Roles>> GetRolesByUserIdAsync(Guid userId);
    Task<bool> UpdateUsersRolesAsync(Guid userid, IEnumerable<string> roleNames);
    Task<bool> UpdateRoleAsync(Guid roleId, string newRoleName);
    Task<IEnumerable<Guid>> GetRolesIdsByNameAsync(IEnumerable<string> roleNames);
    Task<bool> AddRoleAsync(Roles role);
}
