using BE_VetRuta.Data;
using BE_VetRuta.DTOs;
using Microsoft.EntityFrameworkCore;

namespace VetRuta.Api.Services;

public class CatalogosService(VetRutaDbContext db)
{
    public Task<List<ZonaDto>> ListarZonasAsync() =>
        db.Zona.AsNoTracking()
            .OrderBy(z => z.Id)
            .Select(z => new ZonaDto(z.Id, z.Nombre))
            .ToListAsync();

    public async Task<List<EspecieDto>> ListarEspeciesAsync()
    {
        var especies = await db.Especie.AsNoTracking().OrderBy(e => e.Nombre).ToListAsync();
        var razas = await db.Raza.AsNoTracking().OrderBy(r => r.Nombre).ToListAsync();

        return especies
            .Select(e => new EspecieDto(
                e.Id,
                e.Nombre,
                razas.Where(r => r.EspecieId == e.Id).Select(r => new RazaDto(r.Id, r.Nombre)).ToList()))
            .ToList();
    }

    public Task<List<ServicioDto>> ListarServiciosAsync() =>
        db.Servicio.AsNoTracking()
            .OrderBy(s => s.Nombre)
            .Select(s => new ServicioDto(s.Id, s.Nombre, s.Precio, s.DuracionMinutos))
            .ToListAsync();
}
