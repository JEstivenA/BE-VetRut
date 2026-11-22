using BE_VetRuta.Utilitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetRuta.Api.DTOs;
using VetRuta.Api.Services;

namespace VetRuta.Api.Controllers;

[ApiController]
[Route("api/visitas")]
public class VisitasController(VisitasService visitasService) : ControllerBase
{
    
    [Authorize(Roles = Roles.Negocio)]
    [HttpGet]
    public async Task<ActionResult<List<VisitaDto>>> Listar(
        [FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta, [FromQuery] string? estado,
        [FromQuery] int? pacienteId, [FromQuery] int? tecnicoId) =>
        Ok(await visitasService.ListarAsync(User.GetUsuarioId(), User.GetRol(), desde, hasta, estado, pacienteId, tecnicoId));

    [Authorize(Roles = Roles.Negocio)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VisitaDetalleDto>> Obtener(int id) =>
        Ok(await visitasService.ObtenerAsync(id, User.GetUsuarioId(), User.GetRol()));

    [Authorize(Roles = $"{Roles.Medico},{Roles.Tecnico}")]
    [HttpPost]
    public async Task<ActionResult<VisitaDto>> Programar(CrearVisitaRequest request)
    {
        var visita = await visitasService.CrearAsync(request, User.GetUsuarioId(), User.GetRol());
        return Created($"/api/visitas/{visita.Id}", visita);
    }

    [Authorize(Roles = Roles.Medico)]
    [HttpPost("{id:int}/cancelar")]
    public async Task<IActionResult> Cancelar(int id, CancelarVisitaRequest request)
    {
        await visitasService.CancelarAsync(id, request.Motivo, User.GetUsuarioId());
        return NoContent();
    }

    [Authorize(Roles = Roles.Medico)]
    [HttpPost("{id:int}/reprogramar")]
    public async Task<ActionResult<VisitaDto>> Reprogramar(int id, ReprogramarVisitaRequest request) =>
        Ok(await visitasService.ReprogramarAsync(id, request.NuevaFecha, request.Motivo, User.GetUsuarioId()));

    [Authorize(Roles = Roles.Tecnico)]
    [HttpPost("{id:int}/registro")]
    public async Task<ActionResult<VisitaDetalleDto>> Registrar(int id, RegistrarVisitaRequest request) =>
        Ok(await visitasService.RegistrarAsync(id, request, User.GetUsuarioId()));
}
