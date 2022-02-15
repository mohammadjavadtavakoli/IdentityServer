using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace IdentityServer.Identity.UserValidator
{
    public class UserValidator:IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //Just For Test
            var username = context.UserName;
            var password = context.Password;

            if (username=="mohammadjavad" && password=="Tavakoli")
            {
                // context set to success
                context.Result = new GrantValidationResult(
                    subject: username,
                    authenticationMethod: "custom",
                    claims: new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, username)
                    }
                );

                return Task.FromResult(0);
            }

            // context set to Failure        
            context.Result = new GrantValidationResult(
                TokenRequestErrors.UnauthorizedClient,
                "Invalid Crdentials");

            return Task.FromResult(0);
        }
    }
}