namespace PowerFuel.Application.DTOs.Auth;

public record LoginResponse(string Token, string Email, string UserName, string Role, Guid UserId);
