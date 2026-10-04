namespace Core.Configuration;
public class JWTConfiguration
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string ExpiryDays { get; set; }=null!;
}
