using Microsoft.AspNetCore.Identity;

namespace Identity.CustomValidator
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError()
            {
                Code = "InvalidUserName",
                Description = $"Бул {userName} колдонуучу аты туура эмес"
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError()
            {
                Code = "DublicateEmail",
                Description = $"Бул э-почта {email} колдонулуп жатат"
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError()
            {
                Code = "DublicateUserName",
                Description = $"Паролунуз эн аз{length} болушу керек"
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError()
            {
                Code = "DublicateUserName",
                Description = $"Бул колдунуучу аты {userName} колдонулуп жатат"
            };
        }
    }
}