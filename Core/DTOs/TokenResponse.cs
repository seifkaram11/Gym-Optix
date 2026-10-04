namespace Core.DTOs;

public class TokenResponse
{
    public required string AccessToken { get; set; }=null!;
    public required string RefreshToken { get; set; }=null!;
}
