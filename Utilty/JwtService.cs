using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PizzashopMVCProject.Utilty;

public class JwtService
{
    private readonly string? _key;
    private readonly string? _issuer;
    private readonly string? _audience;
    public JwtService(IConfiguration configuration)
    {
        _key = configuration["JwtConfig:Key"];
        _issuer = configuration["JwtConfig:Issuer"];
        _audience = configuration["JwtConfig:Audience"];
    }

    public string GenerateJwtToken(string email, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_key); // Secret Code (Salt)

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                //new Claim("ClaimName", "Dynamic Value for Claim") Custom Claim
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    // Extracts claims from a JWT token.
    public ClaimsPrincipal? GetClaimsFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var claims = new ClaimsIdentity(jwtToken.Claims);
        return new ClaimsPrincipal(claims);
    }

    // Retrieves a specific claim value from a JWT token.
    public string? GetClaimValue(string token, string claimType)
    {
        var claimsPrincipal = GetClaimsFromToken(token);
        // return claimsPrincipal?.FindFirst(claimType)?.Value;
        var value = claimsPrincipal?.FindFirst(claimType)?.Value;
        return value;
    }
}
