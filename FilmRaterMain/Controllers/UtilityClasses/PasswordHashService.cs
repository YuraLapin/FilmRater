using Microsoft.AspNetCore.Identity;

namespace FilmRaterMain.Controllers.UtilityClasses
{
    public class UserDummy
    {

    }

    public class PasswordHashService
    {
        PasswordHasher<UserDummy> passwordHasher;

        public PasswordHashService()
        {
            passwordHasher = new PasswordHasher<UserDummy>();
        }

        public string HashPassword(string password)
        {
            var hashedPassword = passwordHasher.HashPassword(null, password);

            return hashedPassword;
        }

        public bool CheckHash(string storedHash, string unhashedPassword)
        {
            if (passwordHasher.VerifyHashedPassword(null, storedHash, unhashedPassword) == PasswordVerificationResult.Success)
            {
                return true;
            }

            return false;
        }
    }
}
