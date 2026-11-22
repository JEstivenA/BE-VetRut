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

        /// <summary>Técnicos a cargo del médico autenticado.</summary>
        [Authorize(Roles = Roles.Medico)]
        [HttpGet("mis-tecnicos")]
        public async Task<ActionResult<List<TecnicoDto>>> MisTecnicos() =>
            Ok(await usuariosService.ListarMisTecnicosAsync(User.GetUsuarioId()));

        [Authorize(Roles = Roles.Root)]
        [HttpPost]
        public async Task<ActionResult<UsuarioDto>> Crear(CrearUsuarioRequest request)
        {
            var usuario = await usuariosService.CrearAsync(request, User.GetUsuarioId());
            return Created($"/api/usuarios/{usuario.Id}", usuario);
        }


        [Authorize(Roles = Roles.Root)]
        [HttpPatch("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, CambiarEstadoRequest request)
        {
            await usuariosService.CambiarEstadoAsync(id, request.Activo, User.GetUsuarioId());
            return NoContent();
        }
    }
}
