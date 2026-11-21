using BE_VetRuta.Data;
using BE_VetRuta.DTOs;
using BE_VetRuta.Entities;
using BE_VetRuta.Utilitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BE_VetRuta.Services
{
    public class AuthService(VetRutaDbContext db, TokenService tokenService, IOptions<JwtSettings> options)
    {
        private readonly JwtSettings _jwt = options.Value;

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var usuario = await db.Usuario
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == request.Email.Trim());

            if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
                throw ApiException.NoAutorizado("Correo o contraseña incorrectos.");

            if (!usuario.Activo)
                throw ApiException.Prohibido("El usuario está desactivado.");

            return await EmitirTokensAsync(usuario);
        }

        /// <summary>
        /// Entrega un nuevo access token. El refresh token usado se revoca y se
        /// emite uno nuevo, así cada refresh token sirve una sola vez.
        /// </summary>
        public async Task<LoginResponse> RefreshAsync(string refreshToken)
        {
            var actual = await db.RefreshToken
                .Include(r => r.Usuario).ThenInclude(u => u.Rol)
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (actual is null || actual.Revocado || actual.FechaExpiracion < DateTime.Now)
                throw ApiException.NoAutorizado("La sesión expiró. Inicie sesión nuevamente.");

            if (!actual.Usuario.Activo)
                throw ApiException.Prohibido("El usuario está desactivado.");

            actual.Revocado = true;
            return await EmitirTokensAsync(actual.Usuario);
        }

        public async Task LogoutAsync(string refreshToken, int usuarioId)
        {
            var token = await db.RefreshToken
                .FirstOrDefaultAsync(r => r.Token == refreshToken && r.UsuarioId == usuarioId);

            if (token is { Revocado: false })
            {
                token.Revocado = true;
                await db.SaveChangesAsync();
            }
        }

        private async Task<LoginResponse> EmitirTokensAsync(Usuario usuario)
        {
            var (accessToken, expiraEn) = tokenService.GenerarAccessToken(usuario);

            var refresh = new RefreshToken
            {
                UsuarioId = usuario.Id,
                Token = tokenService.GenerarRefreshToken(),
                FechaExpiracion = DateTime.Now.AddDays(_jwt.RefreshTokenDias)
            };
            db.RefreshToken.Add(refresh);
            await db.SaveChangesAsync();

            return new LoginResponse(
                accessToken,
                expiraEn,
                refresh.Token,
                new UsuarioSesionDto(usuario.Id, usuario.Nombre, usuario.Email, usuario.Rol.Nombre));
        }
    }
}
