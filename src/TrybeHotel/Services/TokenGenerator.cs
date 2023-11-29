using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Services
{
    public class TokenGenerator
    {
        private readonly TokenOptions _tokenOptions;
        public TokenGenerator()
        {
           _tokenOptions = new TokenOptions {
                Secret = "4d82a63bbdc67c1e4784ed6587f3730c",
                ExpiresDay = 1
           };

        }
        public string Generate(UserDto user)
        {
        var claims = AddClaims(user);

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenOptions.Secret)),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Expires = DateTime.Now.AddDays(_tokenOptions.ExpiresDay),
            Subject = claims
        };
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
        }

        private static ClaimsIdentity AddClaims(UserDto user)
        {
            Claim nameClain = new(ClaimTypes.Email, user.Email);
            Claim roleClain = new(ClaimTypes.Role, user.UserType);

            ClaimsIdentity claims = new();
            claims.AddClaim(nameClain);
            if (user.UserType == "admin") claims.AddClaim(roleClain);

            return claims;
        }
    }
}