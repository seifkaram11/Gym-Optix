namespace Core.DTOs;

public class AuthenticationResponse
{
  public Guid UserID{get;set;}
  public string? Email{get;set;}
  public string? PersonName{get;set;}
  public string? Gender{get;set;}
  public string? RefreshToken{get;set;}//*
  public string? AccsesToken{get;set;}//*
  public DateTimeOffset RefreshTokenExpiryTime{get;set;}//*
  public bool Success{get;set;}//*
  public AuthenticationResponse() : this(default, default, default,default, default, default,default, default)
  {}

    public AuthenticationResponse(Guid userID, string? email, string? personName, string? gender, string? refreshToken, string? accsesToken, DateTimeOffset refreshTokenExpiryTime, bool success)
    {
        UserID = userID;
        Email = email;
        PersonName = personName;
        Gender = gender;
        RefreshToken = refreshToken;
        AccsesToken = accsesToken;
        RefreshTokenExpiryTime = refreshTokenExpiryTime;
        Success = success;
    }
}
