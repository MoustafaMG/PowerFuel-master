using PowerFuel.Application.Common;

namespace PowerFuel.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    public bool VerifyPassword(string hashedPassword, string providedPassword) => BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
}
