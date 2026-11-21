using BE_VetRuta.DTOs;
using BE_VetRuta.Services;
using BE_VetRuta.Utilitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE_VetRuta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController(UsuariosService usuariosService) : ControllerBase
    {

        [Authorize(Roles = $"{Roles.Root},{Roles.Director}")]
        [HttpGet]
        public async Task<ActionResult<List<UsuarioDto>>> Listar([FromQuery] string? rol) =>
            Ok(await usuariosService.ListarAsync(rol));

        [Authorize(Roles = Roles.Root)]
        [HttpPost]
        public async Task<ActionResult<UsuarioDto>> Crear(CrearUsuarioRequest request)
        {
            var usuario = await usuariosService.CrearAsync(request, User.GetUsuarioId());
            return Created($"/api/usuarios/{usuario.Id}", usuario);
        }

        /// <summary>Activa o desactiva un usuario.</summary>
        [Authorize(Roles = Roles.Root)]
        [HttpPatch("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, CambiarEstadoRequest request)
        {
            await usuariosService.CambiarEstadoAsync(id, request.Activo, User.GetUsuarioId());
            return NoContent();
        }
    }
}
