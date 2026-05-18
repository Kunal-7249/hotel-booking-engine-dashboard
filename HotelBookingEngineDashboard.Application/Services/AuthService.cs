using HotelBookingEngineDashboard.Application.DTOs.Auth;
using HotelBookingEngineDashboard.Application.Exceptions;
using HotelBookingEngineDashboard.Application.Interfaces;
using HotelBookingEngineDashboard.Domain.Entities;
using HotelBookingEngineDashboard.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace HotelBookingEngineDashboard.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IConfiguration configuration,ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            _logger.LogInformation("Registering new user. Email: {Email}", dto.Email);

            var existingEmail = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingEmail is not null)
            {
                _logger.LogWarning("Registration failed. Email already exists: {Email}", dto.Email);
                throw new BadRequestException("Email is already registered.");
            }
            // check if username already exists
            var existingUsername = await _userRepository.GetByUsernameAsync(dto.Username);
            if (existingUsername is not null)
            {
                _logger.LogWarning("Registration failed. Username already taken: {Username}", dto.Username);
                throw new BadRequestException("Username is already taken.");
            }

            if (!Enum.TryParse<UserRole>(dto.Role, out var role))
                role = UserRole.ExternalUser;

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _userRepository.CreateAsync(user);
            
            _logger.LogInformation("User registered successfully. UserId: {UserId}, Role: {Role}",
                created.UserId, created.Role);

            return GenerateToken(created);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            _logger.LogInformation("Login attempt. Email: {Email}", dto.Email);

            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user is null)
            {
                _logger.LogWarning("Login failed. Invalid credentials for Email: {Email}", dto.Email);
                throw new BadRequestException("Invalid email or password.");
            }
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new BadRequestException("Invalid email or password.");

            _logger.LogInformation("Login successful. UserId: {UserId}, Role: {Role}",
            user.UserId, user.Role);

            return GenerateToken(user);
        }

        private AuthResponseDto GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddHours(
                double.Parse(jwtSettings["ExpiryHours"]!));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                ExpiresAt = expiry
            };
        }
    }
}