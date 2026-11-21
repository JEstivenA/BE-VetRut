using BE_VetRuta.Data;
using BE_VetRuta.DTOs;
using BE_VetRuta.Entities;
using BE_VetRuta.Utilitys;
using Microsoft.EntityFrameworkCore;

namespace BE_VetRuta.Services
{
    public class UsuariosService(VetRutaDbContext db, ActividadService actividad)
    {
        // Root crea usuarios de cualquier rol del negocio (no otros root)
        private static readonly string[] RolesCreables = [Roles.Director, Roles.Medico, Roles.Tecnico];

        public async Task<List<UsuarioDto>> ListarAsync(string? rol)
        {
            var query = db.Usuario.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(rol))
            {
                var rolNombre = rol.Trim().ToUpper();
                query = query.Where(u => u.Rol.Nombre == rolNombre);
            }

            return await query
                .OrderBy(u => u.Nombre)
                .Select(u => new UsuarioDto(
                    u.Id, u.Nombre, u.Email, u.Rol.Nombre,
                    u.ZonaId, u.Zona != null ? u.Zona.Nombre : null, u.Activo))
                .ToListAsync();
        }

        public async Task<UsuarioDto> CrearAsync(CrearUsuarioRequest request, int rootId)
        {
            var rolNombre = request.Rol.Trim().ToUpper();
            if (!RolesCreables.Contains(rolNombre))
                throw ApiException.BadRequest("El rol debe ser DIRECTOR, MEDICO o TECNICO.");

            var email = request.Email.Trim().ToLower();

            if (await db.Usuario.AnyAsync(u => u.Email == email))
                throw ApiException.Conflicto("Ya existe un usuario con ese correo.");

            // La zona solo aplica a técnicos y para ellos es obligatoria
            Zona? zona = null;
            if (rolNombre == Roles.Tecnico)
            {
                if (request.ZonaId is null)
                    throw ApiException.BadRequest("La zona es obligatoria para un técnico.");

                zona = await db.Zona.FindAsync(request.ZonaId)
                       ?? throw ApiException.BadRequest("La zona indicada no existe.");
            }

            var rol = await db.Rol.FirstAsync(r => r.Nombre == rolNombre);

            var usuario = new Usuario
            {
                Nombre = request.Nombre.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RolId = rol.Id,
                ZonaId = zona?.Id,
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            db.Usuario.Add(usuario);
            actividad.Registrar(rootId, "USUARIO_CREADO", $"Creó el usuario {usuario.Nombre} con rol {rolNombre}.");
            await db.SaveChangesAsync();

            return new UsuarioDto(usuario.Id, usuario.Nombre, usuario.Email, rolNombre, zona?.Id, zona?.Nombre, true);
        }

        public async Task CambiarEstadoAsync(int usuarioId, bool activo, int rootId)
        {
            if (usuarioId == rootId)
                throw ApiException.BadRequest("No puede cambiar el estado de su propio usuario.");

            var usuario = await db.Usuario.FindAsync(usuarioId)
                          ?? throw ApiException.NoEncontrado("Usuario no encontrado.");

            if (usuario.Activo == activo)
                return;

            usuario.Activo = activo;

            if (!activo)
            {
                // Cierra sus sesiones: ya no podrá renovar el token
                await db.RefreshToken
                    .Where(r => r.UsuarioId == usuarioId && !r.Revocado)
                    .ExecuteUpdateAsync(s => s.SetProperty(r => r.Revocado, true));
            }

            actividad.Registrar(rootId,
                activo ? "USUARIO_ACTIVADO" : "USUARIO_DESACTIVADO",
                $"{(activo ? "Activó" : "Desactivó")} al usuario {usuario.Nombre}.");
            await db.SaveChangesAsync();
        }
    }

}
