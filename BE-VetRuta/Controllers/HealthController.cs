using BE_VetRuta.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE_VetRuta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController(VetRutaDbContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var conectada = await db.Database.CanConnectAsync();
            var usuarios = conectada ? await db.Usuario.CountAsync() : 0;

            return conectada
                ? Ok(new { estado = "ok", usuarios })
                : StatusCode(503, new { estado = "sin conexión a la base de datos" });
        }
    }
}
