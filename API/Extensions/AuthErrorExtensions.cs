using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TestApi.DTOs.ValidationDto;

namespace API.Extensions
{
    public static class AuthErrorExtensions
    {
        public static ModelStateDictionary ToModelState(this List<AuthError> errors)
        {
            var modelstate = new ModelStateDictionary();

            foreach (var error in errors)
            {
                if (error.Field == "InvalidCredentials")
                {
                    modelstate.AddModelError("Username", error.Description);
                    modelstate.AddModelError("Password", error.Description);
                }
                else
                {
                    modelstate.AddModelError(error.Field, error.Description);
                }
            }

            return modelstate;
        }
    }
}
