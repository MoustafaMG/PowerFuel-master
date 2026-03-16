using Microsoft.EntityFrameworkCore;
using PowerFuel.Application.Common;
using PowerFuel.Application.DTOs.Auth;
using PowerFuel.Application.Interfaces;
using PowerFuel.Domain.Entities;
using PowerFuel.Infrastructure.Data;

namespace PowerFuel.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwt;

    public AuthService(ApplicationDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwt)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
            return null;
        var token = _jwt.GenerateToken(user.Id, user.Email, user.UserName, user.Role);
        return new LoginResponse(token, user.Email, user.UserName, user.Role, user.Id);
    }

    public async Task<LoginResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email || u.UserName == request.UserName, cancellationToken))
            return null;
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = "Customer",
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        var token = _jwt.GenerateToken(user.Id, user.Email, user.UserName, user.Role);
        return new LoginResponse(token, user.Email, user.UserName, user.Role, user.Id);
    }
}
