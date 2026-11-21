using BE_VetRuta.Entities;
using BE_VetRuta.Utilitys;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BE_VetRuta.Services
{
    public class TokenService(IOptions<JwtSettings> options)
    {
        private readonly JwtSettings _jwt = options.Value;

        public (string Token, DateTime ExpiraEn) GenerarAccessToken(Usuario usuario)
        {
            var expiraEn = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutos);

            var claims = new[]
            {
                new Claim("sub", usuario.Id.ToString()),
                new Claim("name", usuario.Nombre),
                new Claim("email", usuario.Email),
                new Claim("role", usuario.Rol.Nombre)
            };

            var credenciales = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expiraEn,
                signingCredentials: credenciales);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiraEn);
        }

        public string GenerarRefreshToken() =>
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
