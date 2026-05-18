using FluentAssertions;
using HotelBookingEngineDashboard.Application.DTOs.Auth;
using HotelBookingEngineDashboard.Application.Exceptions;
using HotelBookingEngineDashboard.Application.Interfaces;
using HotelBookingEngineDashboard.Application.Services;
using HotelBookingEngineDashboard.Domain.Entities;
using HotelBookingEngineDashboard.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingEngineDashboard.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly IConfiguration _configuration;
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();

            var logger = new Mock<ILogger<AuthService>>();

            var inMemorySettings = new Dictionary<string, string>
            {
                { "JwtSettings:SecretKey", "HotelBookingDashboard_SuperSecretKey_2026!@#" },
                { "JwtSettings:Issuer", "HotelBookingDashboard" },
                { "JwtSettings:Audience", "HotelBookingDashboardUsers" },
                { "JwtSettings:ExpiryHours", "24" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            _sut = new AuthService(_userRepoMock.Object, _configuration,logger.Object);
        }

        private void SetupUserByEmail(string email, User? user) =>
            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

        private void SetupUserByUsername(string username, User? user) =>
            _userRepoMock.Setup(r => r.GetByUsernameAsync(username)).ReturnsAsync(user);

        private static RegisterDto ValidRegisterDto(
            string username = "kunal",
            string email = "kunal@gmail.com",
            string password = "pass123",
            string role = "ExternalUser") => new()
            {
                Username = username,
                Email = email,
                Password = password,
                Role = role
            };

        private static LoginDto ValidLoginDto(
            string email = "kunal@gmail.com",
            string password = "pass123") => new()
            {
                Email = email,
                Password = password
            };

        private static User ExistingUser(string password = "pass123") => new()
        {
            UserId = 1,
            Username = "kunal",
            Email = "kunal@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = UserRole.ExternalUser
        };

        // RegisterAsync Tests

        [Fact]
        public async Task RegisterAsync_GivenEmailAlreadyExists_ThrowsBadRequestException()
        {
            // Arrange
            SetupUserByEmail("kunal@gmail.com", ExistingUser());

            // Act
            var act = async () => await _sut.RegisterAsync(ValidRegisterDto());

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("*Email*already registered*");
        }

        [Fact]
        public async Task RegisterAsync_GivenUsernameAlreadyTaken_ThrowsBadRequestException()
        {
            // Arrange
            SetupUserByEmail("kunal@gmail.com", null);
            SetupUserByUsername("kunal", ExistingUser());

            // Act
            var act = async () => await _sut.RegisterAsync(ValidRegisterDto());

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("*Username*already taken*");
        }

        [Fact]
        public async Task RegisterAsync_GivenValidData_ReturnsTokenWithCorrectRole()
        {
            // Arrange
            SetupUserByEmail("kunal@gmail.com", null);
            SetupUserByUsername("kunal", null);

            _userRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(new User
                {
                    UserId = 1,
                    Username = "kunal",
                    Email = "kunal@gmail.com",
                    Role = UserRole.ExternalUser
                });

            // Act
            var result = await _sut.RegisterAsync(ValidRegisterDto());

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
            result.Role.Should().Be("ExternalUser");
            result.Username.Should().Be("kunal");
        }

        [Fact]
        public async Task RegisterAsync_GivenAdminRole_ReturnsTokenWithAdminRole()
        {
            // Arrange
            SetupUserByEmail("admin@gmail.com", null);
            SetupUserByUsername("admin", null);

            _userRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(new User
                {
                    UserId = 1,
                    Username = "admin",
                    Email = "admin@gmail.com",
                    Role = UserRole.Admin
                });

            // Act
            var result = await _sut.RegisterAsync(
                ValidRegisterDto(username: "admin", email: "admin@gmail.com", role: "Admin"));

            // Assert
            result.Role.Should().Be("Admin");
        }

        [Fact]
        public async Task RegisterAsync_GivenValidData_CallsCreateRepositoryOnce()
        {
            // Arrange
            SetupUserByEmail("kunal@gmail.com", null);
            SetupUserByUsername("kunal", null);

            _userRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(ExistingUser());

            // Act
            await _sut.RegisterAsync(ValidRegisterDto());

            // Assert
            _userRepoMock.Verify(
                r => r.CreateAsync(It.IsAny<User>()),
                Times.Once);
        }

        //  LoginAsync Tests 

        [Fact]
        public async Task LoginAsync_GivenUserNotFound_ThrowsBadRequestException()
        {
            // Arrange
            SetupUserByEmail("wrong@gmail.com", null);

            // Act
            var act = async () => await _sut.LoginAsync(
                ValidLoginDto(email: "wrong@gmail.com"));

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("*Invalid email or password*");
        }

        [Fact]
        public async Task LoginAsync_GivenWrongPassword_ThrowsBadRequestException()
        {
            // Arrange
            SetupUserByEmail("kunal@gmail.com", ExistingUser("correctpassword"));

            // Act
            var act = async () => await _sut.LoginAsync(
                ValidLoginDto(password: "wrongpassword"));

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("*Invalid email or password*");
        }

        [Fact]
        public async Task LoginAsync_GivenValidCredentials_ReturnsTokenWithCorrectRole()
        {
            // Arrange
            SetupUserByEmail("kunal@gmail.com", new User
            {
                UserId = 1,
                Username = "kunal",
                Email = "kunal@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass123"),
                Role = UserRole.Admin
            });

            // Act
            var result = await _sut.LoginAsync(ValidLoginDto());

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
            result.Role.Should().Be("Admin");
        }

        [Fact]
        public async Task LoginAsync_GivenValidCredentials_TokenExpiresIn24Hours()
        {
            // Arrange
            SetupUserByEmail("kunal@gmail.com", ExistingUser());

            // Act
            var result = await _sut.LoginAsync(ValidLoginDto());

            // Assert
            result.ExpiresAt.Should().BeCloseTo(
                DateTime.UtcNow.AddHours(24),
                precision: TimeSpan.FromMinutes(1));
        }
    }
}
