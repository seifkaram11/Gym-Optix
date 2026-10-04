namespace Core.DTOs;

public class AddRoleToUserRequest
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
