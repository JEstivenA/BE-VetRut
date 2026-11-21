using BE_VetRuta.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetRuta.Api.Services;

namespace VetRuta.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CatalogosController(CatalogosService catalogosService) : ControllerBase
{
    [HttpGet("zonas")]
    public async Task<ActionResult<List<ZonaDto>>> Zonas() =>
        Ok(await catalogosService.ListarZonasAsync());

    [HttpGet("especies")]
    public async Task<ActionResult<List<EspecieDto>>> Especies() =>
        Ok(await catalogosService.ListarEspeciesAsync());

    [HttpGet("servicios")]
    public async Task<ActionResult<List<ServicioDto>>> Servicios() =>
        Ok(await catalogosService.ListarServiciosAsync());
}
