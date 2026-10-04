using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Enum;
using Core.RepositoryContracts;
using Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;

namespace Core.Services;

public class UsersService : IUsersService
{
    IUsersRepository _usersRepository;
    IMapper _mapper;

    public UsersService(IUsersRepository usersRepository, IMapper mapper)
    {
        _usersRepository = usersRepository;
        _mapper = mapper;
    }
    public async Task<bool> AddRoleAsync(AddRoleToUserRequest role)
    {
        if(role.RoleId == Guid.Empty) return false;
        return await _usersRepository.AddRoleToUserAsync(role.UserId, role.RoleId);
    }
}
