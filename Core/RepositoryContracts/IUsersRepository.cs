using Core.Entities;

namespace Core.RepositoryContracts;

public interface IUsersRepository
{
    Task<Users?> AddUserAsync(Users? user);
    Task<IEnumerable<Users>> GetAllUsersAsync();
    Task<Users?> UpdateUserAsync(Users user);
    Task<bool> DeleteUserAsync(Guid userId);
    Task<bool> AddRoleToUserAsync(Guid userId, Guid roleId);
    Task<Users?> GetUserByEmailAsync(string email);
    Task<Users?> GetUserByIDAsync(Guid userID);
}
