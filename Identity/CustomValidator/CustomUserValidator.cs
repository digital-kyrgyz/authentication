using Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Identity.CustomValidator
{
    public class CustomUserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            List<IdentityError> errors = new List<IdentityError>();

            string[] digits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            foreach (var item in digits)
            {
                if (user.UserName[0].ToString() == item)
                {
                    errors.Add(new IdentityError() { Code = "UserNameContainsFirstLetterDigitalContains", Description = "User Name does not contain digital characters" });
                }
            }
            if (errors.Count == 0)
            {
                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
        }
    }
}