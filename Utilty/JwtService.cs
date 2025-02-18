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
}
