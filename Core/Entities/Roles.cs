namespace Core.Entities;

public class Roles
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; }=null!;
    public ICollection<Users> Users { get; set; } = new List<Users>();
}
