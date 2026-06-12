using Application.DTOs;
using BCrypt.Net;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Repositories;
using Infrastructure.Auth;
namespace Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenService _jwt;

    public AuthService(IUserRepository userRepository, JwtTokenService jwt)
    {
        _userRepository = userRepository;
        _jwt = jwt;
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        var existing = await _userRepository.GetByEmailAsync(dto.Email);
        if (existing != null)
            throw new Exception("Email already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return _jwt.GenerateToken(user);
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid credentials");

        return _jwt.GenerateToken(user);
    }
}