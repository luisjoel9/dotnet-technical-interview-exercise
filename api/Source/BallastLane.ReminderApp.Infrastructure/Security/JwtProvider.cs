using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BallastLane.ReminderApp.Application.Interfaces;
using BallastLane.ReminderApp.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace BallastLane.ReminderApp.Infrastructure.Security
{
    public class JwtProvider : IJwtProvider
    {
        private readonly string _secretKey = "SUPER_SECRET_KEY_FOR_TECH_INTERVIEW";
        private readonly string _issuer = "ReminderAppBackend";
        private readonly string _audience = "ReminderAppFrontend";

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim("custom_claim_project", "TechChallenge")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(2);

            var tokenOptions = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}
