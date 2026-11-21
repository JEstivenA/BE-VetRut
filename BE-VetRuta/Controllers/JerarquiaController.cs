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
    [Authorize(Roles = Roles.Director)]
    public class JerarquiaController(JerarquiaService jerarquiaService) : ControllerBase
    {
        /// <summary>Médicos con sus técnicos y técnicos sin médico.</summary>
        [HttpGet]
        public async Task<ActionResult<JerarquiaDto>> Obtener() =>
            Ok(await jerarquiaService.ObtenerAsync());

        /// <summary>Asigna o reasigna un técnico a un médico.</summary>
        [HttpPut("tecnicos/{tecnicoId:int}/medico")]
        public async Task<ActionResult<AsignacionResultadoDto>> AsignarTecnico(int tecnicoId, AsignarTecnicoRequest request) =>
            Ok(await jerarquiaService.AsignarTecnicoAsync(tecnicoId, request.MedicoId, User.GetUsuarioId()));
    }
}
