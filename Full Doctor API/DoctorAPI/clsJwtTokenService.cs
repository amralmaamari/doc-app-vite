using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
namespace DoctorAPI
{
    public class clsJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public clsJwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(int userId)
        {
            // Get settings from configuration
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var durationInMinutes = int.Parse(jwtSettings["DurationInMinutes"]);

            // Create key and credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Set claims (information in the token)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, userId.ToString()) // Use NameId for a unique user identifier

        };

            // Create the token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(durationInMinutes),
                signingCredentials: credentials);

            // Return the token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateToken(string email)
        {
            // Get settings from configuration
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var durationInMinutes = int.Parse(jwtSettings["DurationInMinutes"]);

            // Create key and credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Set claims (information in the token)
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Email, email) // Use Email for a unique user identifier

        };

            // Create the token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(durationInMinutes),
                signingCredentials: credentials);

            // Return the token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static int GetUserToken(ControllerBase controller)
        {
            // Get the user claim for name identifier
            var userIdClaim = controller.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            // Check if the claim exists and parse it as an integer
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            // Return -1 or throw an exception if the claim was not found or could not be parsed
            throw new InvalidOperationException("User ID claim not found or invalid.");
        }

    }
}
