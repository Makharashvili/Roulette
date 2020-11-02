using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Helpers
{
    public static class TokenHelper
    {
        public static string CreateToken(string key, DateTime expires, params Claim[] claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var bytes = Encoding.ASCII.GetBytes(key);

            var desc = new SecurityTokenDescriptor
            {
                Expires = expires,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(bytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenObject = tokenHandler.CreateToken(desc);

            return tokenHandler.WriteToken(tokenObject);
        }
    }
}
