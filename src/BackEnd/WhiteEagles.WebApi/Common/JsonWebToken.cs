namespace WhiteEagles.WebApi.Common
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    using Data.DomainModels;

    public class JsonWebToken
    {
        private readonly IConfiguration _config;

        public JsonWebToken(IConfiguration config)
            => _config = config ?? throw new ArgumentNullException(nameof(config));

        public string GenerateJwtToken(UserInfo user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("code", user.Code),
                new Claim("id", user.Id),
                new Claim(ClaimTypes.Role, user.RoleCode),
                new Claim(ClaimTypes.Email, user.EMail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
