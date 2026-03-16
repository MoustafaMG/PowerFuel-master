namespace PowerFuel.Application.Common;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email, string userName, string role);
}
