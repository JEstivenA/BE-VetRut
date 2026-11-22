using BE_VetRuta.DTOs;
using BE_VetRuta.Services;
using BE_VetRuta.Utilitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE_VetRuta.Controllers;

/// <summary>
/// Subida de fotos (paciente y visita). La app primero sube la imagen aquí y
/// luego envía la URL devuelta en el formulario correspondiente.
/// </summary>
[ApiController]
[Route("api/archivos")]
[Authorize(Roles = $"{Roles.Medico},{Roles.Tecnico}")]
public class ArchivosController(ArchivosService archivosService) : ControllerBase
{
    [HttpPost("imagenes")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ArchivoDto>> SubirImagen(IFormFile archivo) =>
        Ok(new ArchivoDto(await archivosService.GuardarImagenAsync(archivo)));
}
