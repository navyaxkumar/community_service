using DigitalShield.API.Configuration;
using DigitalShield.API.Data;
using DigitalShield.API.DTOs;
using DigitalShield.API.Models;
using DigitalShield.API.Repositories;
using DigitalShield.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ReturnsSuccessWithToken()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);

        var user = new User
        {
            Name = "Alice",
            Email = "alice@example.com",
            PasswordHash = new PasswordHasher<User>().HashPassword(new User(), "Password123!")
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new AuthService(
            new UserRepository(context),
            new JwtSettings
            {
                Issuer = "DigitalShield",
                Audience = "DigitalShieldUsers",
                SecretKey = "ThisIsAStrongSecretKeyForTests123!",
                ExpiryMinutes = 60
            });

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "alice@example.com",
            Password = "Password123!"
        });

        Assert.True(result.Success);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal("Alice", result.UserName);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalid_ReturnsFailure()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);

        context.Users.Add(new User
        {
            Name = "Bob",
            Email = "bob@example.com",
            PasswordHash = new PasswordHasher<User>().HashPassword(new User(), "CorrectPass123!")
        });
        await context.SaveChangesAsync();

        var service = new AuthService(
            new UserRepository(context),
            new JwtSettings
            {
                Issuer = "DigitalShield",
                Audience = "DigitalShieldUsers",
                SecretKey = "ThisIsAStrongSecretKeyForTests123!",
                ExpiryMinutes = 60
            });

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "bob@example.com",
            Password = "WrongPass123!"
        });

        Assert.False(result.Success);
        Assert.Null(result.Token);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailIsUnique_CreatesUserAndReturnsSuccess()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);

        var service = new AuthService(
            new UserRepository(context),
            new JwtSettings
            {
                Issuer = "DigitalShield",
                Audience = "DigitalShieldUsers",
                SecretKey = "ThisIsAStrongSecretKeyForTests123!",
                ExpiryMinutes = 60
            });

        var result = await service.RegisterAsync(new RegisterRequest
        {
            Name = "Charlie",
            Email = "charlie@example.com",
            Password = "StrongPass123!"
        });

        Assert.True(result.Success);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal("Charlie", result.UserName);
        Assert.Equal(1, context.Users.Count());
    }
}
