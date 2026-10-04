using Core.DTOs;
using Core.Entities;
using Core.Enum;

namespace Core.ServiceContracts;

public interface IUsersService
{
    Task<bool> AddRoleAsync(AddRoleToUserRequest role);
}
