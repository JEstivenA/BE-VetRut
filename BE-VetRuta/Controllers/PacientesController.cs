using BE_VetRuta.DTOs;
using BE_VetRuta.Services;
using BE_VetRuta.Utilitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE_VetRuta.Controllers;

[ApiController]
[Route("api/pacientes")]
public class PacientesController(PacientesService pacientesService) : ControllerBase
{
    /// <summary>Pacientes visibles según el rol (también alimenta el mapa).</summary>
    [Authorize(Roles = Roles.Negocio)]
    [HttpGet]
    public async Task<ActionResult<List<PacienteListaDto>>> Listar() =>
        Ok(await pacientesService.ListarAsync(User.GetUsuarioId(), User.GetRol()));

    /// <summary>Detalle con historial de visitas.</summary>
    [Authorize(Roles = Roles.Negocio)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PacienteDetalleDto>> Obtener(int id) =>
        Ok(await pacientesService.ObtenerAsync(id, User.GetUsuarioId(), User.GetRol()));

    [Authorize(Roles = Roles.Medico)]
    [HttpPost]
    public async Task<ActionResult<PacienteDetalleDto>> Crear(CrearPacienteRequest request)
    {
        var paciente = await pacientesService.CrearAsync(request, User.GetUsuarioId());
        return Created($"/api/pacientes/{paciente.Id}", paciente);
    }

    [Authorize(Roles = Roles.Medico)]
    [HttpPut("{id:int}/tecnico")]
    public async Task<ActionResult<AsignacionPacienteResultadoDto>> AsignarTecnico(int id, AsignarTecnicoPacienteRequest request) =>
        Ok(await pacientesService.AsignarTecnicoAsync(id, request.TecnicoId, User.GetUsuarioId()));
}
