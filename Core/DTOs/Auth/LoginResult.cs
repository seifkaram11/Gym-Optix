namespace TestApi.DTOs.ValidationDto
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpirationUtc { get; set; }

        public AuthError? Error { get; set; } 
        public static LoginResult Success(string token, DateTime expires)
            => new LoginResult { Succeeded = true, Token = token, ExpirationUtc = expires};
        public static LoginResult Failure(string field, string description) 
            => new LoginResult { Succeeded = false, Error=new AuthError(field,description) };
    }
}

