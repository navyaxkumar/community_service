using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DigitalShield.API.Authentication;
using DigitalShield.API.Configuration;
using DigitalShield.API.Data;
using DigitalShield.API.DTOs.User;
using DigitalShield.API.Models;
using DigitalShield.API.Repositories;
using DigitalShield.API.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DigitalShield.Tests;

public class AuthenticationServiceTests
{
    private const string SecretKey = "test-only-secret-key-that-is-at-least-32-bytes-long";
    private const string Issuer = "DigitalShield.Tests";
    private const string Audience = "DigitalShield.Tests.Client";

    [Fact]
    public async Task RegisterAsync_HashesPasswordAndReturnsSafeUser()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.RegisterAsync(new RegisterRequestDto
        {
            Name = "Alice",
            Email = " Alice@Example.com ",
            Password = "StrongPass1"
        });

        var storedUser = await context.Users.SingleAsync();
        Assert.Equal("alice@example.com", storedUser.Email);
        Assert.NotEqual("StrongPass1", storedUser.PasswordHash);
        Assert.True(new PasswordHasherService().VerifyPassword(storedUser.PasswordHash, "StrongPass1"));
        Assert.Equal("User", result.Role);
        Assert.Null(result.GetType().GetProperty("PasswordHash"));
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmailIsRejectedAfterNormalization()
    {
        await using var context = CreateContext();
        var service = CreateService(context);
        await service.RegisterAsync(new RegisterRequestDto
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "StrongPass1"
        });

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegisterAsync(new RegisterRequestDto
        {
            Name = "Another Alice",
            Email = " ALICE@EXAMPLE.COM ",
            Password = "StrongPass1"
        }));
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentialsReturnsTokenAndClaims()
    {
        await using var context = CreateContext();
        var service = CreateService(context);
        await service.RegisterAsync(new RegisterRequestDto
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "StrongPass1"
        });

        var result = await service.LoginAsync(new LoginRequestDto
        {
            Email = " ALICE@EXAMPLE.COM ",
            Password = "StrongPass1"
        });

        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);
        Assert.Equal("1", token.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal("alice@example.com", token.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal(Audience, token.Audiences.Single());
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_InvalidPasswordUnknownEmailAndInactiveUserUseGenericFailure()
    {
        await using var context = CreateContext();
        var service = CreateService(context);
        await service.RegisterAsync(new RegisterRequestDto
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "StrongPass1"
        });

        await Assert.ThrowsAsync<AuthenticationException>(() => service.LoginAsync(new LoginRequestDto
        {
            Email = "alice@example.com",
            Password = "WrongPass1"
        }));
        await Assert.ThrowsAsync<AuthenticationException>(() => service.LoginAsync(new LoginRequestDto
        {
            Email = "unknown@example.com",
            Password = "WrongPass1"
        }));

        var user = await context.Users.SingleAsync();
        user.IsActive = false;
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<AuthenticationException>(() => service.LoginAsync(new LoginRequestDto
        {
            Email = "alice@example.com",
            Password = "StrongPass1"
        }));
    }

    [Fact]
    public void RegisterRequestValidator_RejectsWeakPassword()
    {
        var result = new RegisterRequestDtoValidator().Validate(new RegisterRequestDto
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "weak"
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void JwtValidation_RejectsTamperedSignatureWrongIssuerAudienceAndExpiredToken()
    {
        var tokenService = new JwtTokenService(Options.Create(CreateSettings()));
        var token = tokenService.CreateToken(new User
        {
            Id = 1,
            Name = "Alice",
            Email = "alice@example.com",
            Role = "User"
        }).AccessToken;

        var validParameters = CreateValidationParameters(SecretKey, Issuer, Audience);
        var handler = new JwtSecurityTokenHandler();
        handler.ValidateToken(token, validParameters, out _);

        Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() =>
            handler.ValidateToken(token, CreateValidationParameters("wrong-key-that-is-also-long-enough-123", Issuer, Audience), out _));
        Assert.Throws<SecurityTokenInvalidIssuerException>(() =>
            handler.ValidateToken(token, CreateValidationParameters(SecretKey, "wrong-issuer", Audience), out _));
        Assert.Throws<SecurityTokenInvalidAudienceException>(() =>
            handler.ValidateToken(token, CreateValidationParameters(SecretKey, Issuer, "wrong-audience"), out _));

        var expiredToken = CreateExpiredToken();
        Assert.Throws<SecurityTokenExpiredException>(() =>
            handler.ValidateToken(expiredToken, validParameters, out _));
    }

    [Fact]
    public void JwtSettings_RejectMissingOrWeakSecret()
    {
        var settings = new JwtSettings
        {
            Issuer = Issuer,
            Audience = Audience,
            SecretKey = "short",
            ExpiryMinutes = 60
        };

        Assert.Throws<InvalidOperationException>(() => settings.Validate());
    }

    private static AuthenticationService CreateService(ApplicationDbContext context)
    {
        return new AuthenticationService(
            new UserRepository(context),
            new PasswordHasherService(),
            new JwtTokenService(Options.Create(CreateSettings())));
    }

    private static JwtSettings CreateSettings()
    {
        return new JwtSettings
        {
            Issuer = Issuer,
            Audience = Audience,
            SecretKey = SecretKey,
            ExpiryMinutes = 60
        };
    }

    private static TokenValidationParameters CreateValidationParameters(string secret, string issuer, string audience)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    private static string CreateExpiredToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: new[] { new Claim(JwtRegisteredClaimNames.Sub, "1") },
            expires: DateTime.UtcNow.AddMinutes(-5),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
}
