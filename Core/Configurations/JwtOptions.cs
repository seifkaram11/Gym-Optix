namespace Core.Configuration;
public class JwtOptions
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string SecurityKey { get; set; } = null!;
    public double ExpiryMinutes { get; set; }
}
