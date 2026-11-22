using BE_VetRuta.Data;
using BE_VetRuta.Entities;
using BE_VetRuta.Services;
using BE_VetRuta.Utilitys;
using Microsoft.EntityFrameworkCore;
using VetRuta.Api.DTOs;

namespace VetRuta.Api.Services;

public class VisitasService(VetRutaDbContext db, AlcanceService alcance, ActividadService actividad)
{
    public async Task<List<VisitaDto>> ListarAsync(
        int usuarioId, string rol,
        DateOnly? desde, DateOnly? hasta, string? estado, int? pacienteId, int? tecnicoId)
    {
        var query = alcance.Visitas(usuarioId, rol);

        if (desde is not null)
        {
            var inicio = desde.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(v => v.FechaProgramada >= inicio);
        }
        if (hasta is not null)
        {
            var fin = hasta.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            query = query.Where(v => v.FechaProgramada < fin);
        }
        if (pacienteId is not null)
            query = query.Where(v => v.PacienteId == pacienteId);
        if (tecnicoId is not null)
            query = query.Where(v => v.TecnicoId == tecnicoId);

        var hoy = DateTime.Today;
        query = estado?.Trim().ToUpper() switch
        {
            null or "" => query,
            EstadosVisita.Atrasada => query.Where(v => v.Estado == EstadosVisita.Pendiente && v.FechaProgramada < hoy),
            EstadosVisita.Pendiente => query.Where(v => v.Estado == EstadosVisita.Pendiente && v.FechaProgramada >= hoy),
            EstadosVisita.Realizada => query.Where(v => v.Estado == EstadosVisita.Realizada),
            EstadosVisita.Cancelada => query.Where(v => v.Estado == EstadosVisita.Cancelada),
            _ => throw ApiException.BadRequest("Estado no válido. Use PENDIENTE, ATRASADA, REALIZADA o CANCELADA.")
        };

        return await ProyectarAsync(query.OrderBy(v => v.FechaProgramada));
    }

    public async Task<VisitaDetalleDto> ObtenerAsync(int visitaId, int usuarioId, string rol)
    {
        var visita = (await ProyectarAsync(alcance.Visitas(usuarioId, rol).Where(v => v.Id == visitaId)))
                     .FirstOrDefault()
                     ?? throw ApiException.NoEncontrado("Visita no encontrada.");

        var registro = await db.RegistroVisita.AsNoTracking()
            .Where(r => r.VisitaId == visitaId)
            .Select(r => new RegistroVisitaDto(
                r.Peso, r.Altura, r.Temperatura, r.CondicionCorporal, r.Observaciones,
                r.Latitud, r.Longitud, r.FotoUrl, r.FechaRegistro))
            .FirstOrDefaultAsync();

        return new VisitaDetalleDto(visita, registro);
    }

    public async Task<VisitaDto> CrearAsync(CrearVisitaRequest request, int usuarioId, string rol)
    {
        var paciente = await alcance.Pacientes(usuarioId, rol).FirstOrDefaultAsync(p => p.Id == request.PacienteId)
                       ?? throw ApiException.NoEncontrado("Paciente no encontrado.");

        if (!await db.Servicio.AnyAsync(s => s.Id == request.ServicioId))
            throw ApiException.BadRequest("El servicio no existe.");

        ValidarFechaFutura(request.FechaProgramada);

        var tecnicoId = await TecnicoVigenteAsync(paciente.Id);

        var visita = new Visita
        {
            PacienteId = paciente.Id,
            ServicioId = request.ServicioId,
            TecnicoId = tecnicoId,
            FechaProgramada = request.FechaProgramada,
            Estado = EstadosVisita.Pendiente,
            CreadaPorId = usuarioId,
            FechaCreacion = DateTime.Now
        };
        db.Visita.Add(visita);
        actividad.Registrar(usuarioId, "VISITA_PROGRAMADA",
            $"Programó una visita para {paciente.Nombre} el {request.FechaProgramada:dd/MM/yyyy HH:mm}.");
        await db.SaveChangesAsync();

        return await ObtenerDtoAsync(visita.Id);
    }

    public async Task CancelarAsync(int visitaId, string motivo, int medicoId)
    {
        var visita = await ObtenerPendienteDelMedicoAsync(visitaId, medicoId);

        visita.Estado = EstadosVisita.Cancelada;
        visita.MotivoCancelacion = motivo.Trim();
        actividad.Registrar(medicoId, "VISITA_CANCELADA", $"Canceló la visita #{visita.Id}: {motivo.Trim()}");
        await db.SaveChangesAsync();
    }

    
    public async Task<VisitaDto> ReprogramarAsync(int visitaId, DateTime nuevaFecha, string motivo, int medicoId)
    {
        var original = await ObtenerPendienteDelMedicoAsync(visitaId, medicoId);
        ValidarFechaFutura(nuevaFecha);
        var tecnicoId = await TecnicoVigenteAsync(original.PacienteId);

        original.Estado = EstadosVisita.Cancelada;
        original.MotivoCancelacion = $"Reprogramada: {motivo.Trim()}";

        var nueva = new Visita
        {
            PacienteId = original.PacienteId,
            ServicioId = original.ServicioId,
            TecnicoId = tecnicoId,
            FechaProgramada = nuevaFecha,
            Estado = EstadosVisita.Pendiente,
            VisitaOrigenId = original.Id,
            CreadaPorId = medicoId,
            FechaCreacion = DateTime.Now
        };
        db.Visita.Add(nueva);
        actividad.Registrar(medicoId, "VISITA_REPROGRAMADA",
            $"Reprogramó la visita #{original.Id} para el {nuevaFecha:dd/MM/yyyy HH:mm}.");
        await db.SaveChangesAsync();

        return await ObtenerDtoAsync(nueva.Id);
    }

    
    public async Task<VisitaDetalleDto> RegistrarAsync(int visitaId, RegistrarVisitaRequest request, int tecnicoId)
    {
        var visita = await db.Visita
            .Include(v => v.Paciente)
            .FirstOrDefaultAsync(v => v.Id == visitaId && v.TecnicoId == tecnicoId)
            ?? throw ApiException.NoEncontrado("Visita no encontrada.");

        if (visita.Estado != EstadosVisita.Pendiente)
            throw ApiException.Conflicto("La visita ya fue registrada o cancelada; su información es histórica.");

        if (visita.FechaProgramada.Date > DateTime.Today)
            throw ApiException.BadRequest("No puede registrar una visita antes de su fecha programada.");

        var atrasada = EstadosVisita.Calcular(visita.Estado, visita.FechaProgramada) == EstadosVisita.Atrasada;

        db.RegistroVisita.Add(new RegistroVisita
        {
            VisitaId = visita.Id,
            Peso = request.Peso,
            Altura = request.Altura,
            Temperatura = request.Temperatura,
            CondicionCorporal = request.CondicionCorporal,
            Observaciones = request.Observaciones?.Trim(),
            Latitud = request.Latitud,
            Longitud = request.Longitud,
            FotoUrl = request.FotoUrl,
            FechaRegistro = DateTime.Now
        });
        visita.Estado = EstadosVisita.Realizada;

        actividad.Registrar(tecnicoId, "VISITA_REGISTRADA",
            $"Registró la visita de {visita.Paciente.Nombre}{(atrasada ? " (con atraso)" : "")}.");
        await db.SaveChangesAsync();

        return await ObtenerAsync(visita.Id, tecnicoId, Roles.Tecnico);
    }

    
    private async Task<Visita> ObtenerPendienteDelMedicoAsync(int visitaId, int medicoId)
    {
        var visita = await alcance.Visitas(medicoId, Roles.Medico).FirstOrDefaultAsync(v => v.Id == visitaId)
                     ?? throw ApiException.NoEncontrado("Visita no encontrada.");

        if (visita.Estado != EstadosVisita.Pendiente)
            throw ApiException.Conflicto("Solo se pueden modificar visitas pendientes o atrasadas.");

        return visita;
    }

    private async Task<int> TecnicoVigenteAsync(int pacienteId) =>
        await db.AsignacionPacienteTecnico
            .Where(a => a.PacienteId == pacienteId && a.FechaFin == null)
            .Select(a => (int?)a.TecnicoId)
            .FirstOrDefaultAsync()
        ?? throw ApiException.BadRequest("El paciente no tiene un técnico asignado.");

    private static void ValidarFechaFutura(DateTime fecha)
    {
        if (fecha.Date < DateTime.Today)
            throw ApiException.BadRequest("La fecha programada no puede ser anterior a hoy.");
    }

    private async Task<VisitaDto> ObtenerDtoAsync(int visitaId) =>
        (await ProyectarAsync(db.Visita.Where(v => v.Id == visitaId))).First();

    private static async Task<List<VisitaDto>> ProyectarAsync(IQueryable<Visita> query)
    {
        var datos = await query.AsNoTracking()
            .Select(v => new
            {
                v.Id,
                v.PacienteId,
                Paciente = v.Paciente.Nombre,
                Especie = v.Paciente.Especie.Nombre,
                v.ServicioId,
                Servicio = v.Servicio.Nombre,
                v.TecnicoId,
                Tecnico = v.Tecnico.Nombre,
                v.FechaProgramada,
                v.Estado,
                v.MotivoCancelacion,
                v.VisitaOrigenId,
                v.Paciente.Dueno.Direccion,
                v.Paciente.Dueno.Latitud,
                v.Paciente.Dueno.Longitud
            })
            .ToListAsync();

        return datos.Select(d => new VisitaDto(
            d.Id, d.PacienteId, d.Paciente, d.Especie, d.ServicioId, d.Servicio,
            d.TecnicoId, d.Tecnico, d.FechaProgramada,
            EstadosVisita.Calcular(d.Estado, d.FechaProgramada),
            d.MotivoCancelacion, d.VisitaOrigenId, d.Direccion, d.Latitud, d.Longitud)).ToList();
    }
}
