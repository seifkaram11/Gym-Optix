using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TestApi.DTOs.ValidationDto
{
    public class AuthError
    {
        public string Field { get; set; }
        public string Description { get; set; }
        public AuthError() { }
        public AuthError(string Field, string Description)
        {
            this.Field = Field;
            this.Description = Description;
        }
    }

}
