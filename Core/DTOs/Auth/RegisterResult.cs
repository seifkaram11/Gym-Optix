namespace TestApi.DTOs.ValidationDto
{
    public class RegisterResult
    {
        public bool Succeeded { get; set; }
        public List<AuthError>? Errors { get; set; } = new List<AuthError>();

        public static RegisterResult Success() 
            => new RegisterResult { Succeeded = true };
        public static RegisterResult Failure(List<AuthError> errors)
            => new RegisterResult { Succeeded = false, Errors = errors };
    }
}
