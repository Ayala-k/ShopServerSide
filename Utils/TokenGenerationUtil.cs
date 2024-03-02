using Microsoft.IdentityModel.Tokens;
using serverSide.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace serverSide.Utils;

public static class TokenGenerationUtil
{
    public static string GenerateJwtToken(int userId, Roles role, IConfiguration _configuration)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Role, role.ToString()) // Add role claim
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            // Add audience and issuer claims
            Claims = new Dictionary<string, object>
    {
        { JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"] },
        { JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"] }
    }
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

}
