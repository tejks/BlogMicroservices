using System.IdentityModel.Tokens.Jwt;
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthAPI.Models;
using Core.Configuration;
using Core.Entities.Models;
using JWT.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Core.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }
    
    public string GenerateAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expirationTimeStamp = DateTime.Now.AddMinutes(15);
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(ClaimTypes.Role, Enum.GetName(user.Role))
        };

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: expirationTimeStamp,
            signingCredentials: credentials
        );
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        
        return tokenString;
    }

    public RefreshToken GenerateRefreshToken(User user)
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            
            return new RefreshToken
            {
                UserId = user.Id,
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(10),
                CreatedDate = DateTime.UtcNow,
            };
        }
    }
    
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }
        
        return principal;
    }

    public bool Verify(string token)
    {
        var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config["JwtSettings:Issuer"],
            ValidAudience = _config["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.FromSeconds(60)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        SecurityToken validatedToken;
        try
        {
            tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
        }
        catch (Exception)
        {
            return false;
        }

        return validatedToken != null;
    }
}