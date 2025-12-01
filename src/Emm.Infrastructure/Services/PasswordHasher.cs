using Emm.Application.Abstractions;

namespace Emm.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        // Implement hashing logic here, e.g., using BCrypt or another hashing algorithm
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        // Verify the provided password against the hashed password
        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }
}
