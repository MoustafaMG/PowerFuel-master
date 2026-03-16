namespace PowerFuel.Application.DTOs.Auth;

public record RegisterRequest(string UserName, string Email, string Password, string? FirstName, string? LastName);
