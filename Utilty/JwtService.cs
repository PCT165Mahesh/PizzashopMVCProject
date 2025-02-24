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

    public string GenerateJwtToken(string email, string role, string userName, string imgUrl)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_key); // Secret Code (Salt)

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("email", email),
                new Claim("role", role),
                new Claim("userName", userName),
                new Claim("imgUrl", imgUrl)
                //new Claim("ClaimName", "Dynamic Value for Claim") Custom Claim
            }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }



     // ✅ Validate the Token
    public bool ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Convert.FromBase64String(_key); // Convert key to byte array

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, 
                ValidateAudience = false, 
                ValidateLifetime = true, 
                ClockSkew = TimeSpan.Zero 
            }, out _);

            return true; // Token is valid
        }
        catch
        {
            return false; //Token is invalid
        }
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