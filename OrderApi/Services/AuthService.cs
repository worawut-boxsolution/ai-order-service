using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.DTOs;
using OrderApi.Models;

namespace OrderApi.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(string phone, string password);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AuthResponse> LoginAsync(string phone, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone && u.Password == password);
        
        if (user == null)
            return new AuthResponse(false, "Invalid credentials", null, null);

        // Simple JWT token generation mock-up
        var token = "mock-jwt-token-" + Guid.NewGuid().ToString();
        
        return new AuthResponse(true, "Login successful", user, token);
    }
}
