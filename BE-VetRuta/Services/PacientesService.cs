using BE_VetRuta.Data;
using BE_VetRuta.DTOs;
using BE_VetRuta.Utilitys;
using BE_VetRuta.Entities;

using Microsoft.EntityFrameworkCore;

namespace BE_VetRuta.Services;

public class PacientesService(VetRutaDbContext db, AlcanceService alcance, ActividadService actividad)
{
    public async Task<PacienteDetalleDto> CrearAsync(CrearPacienteRequest request, int medicoId)
    {
        var raza = await db.Raza.FindAsync(request.RazaId);
        if (raza is null || raza.EspecieId != request.EspecieId)
            throw ApiException.BadRequest("La raza no corresponde a la especie seleccionada.");

        if (request.FechaNacimientoAprox > DateOnly.FromDateTime(DateTime.Today))
            throw ApiException.BadRequest("La fecha de nacimiento no puede ser futura.");

        if (request.TecnicoId is not null)
            await ValidarTecnicoDelMedicoAsync(request.TecnicoId.Value, medicoId);

        var dueno = new Dueno
        {
            Nombre = request.Dueno.Nombre.Trim(),
            Telefono = request.Dueno.Telefono.Trim(),
            Direccion = request.Dueno.Direccion.Trim(),
            Latitud = request.Dueno.Latitud,
            Longitud = request.Dueno.Longitud
        };

        var paciente = new Paciente
        {
            Nombre = request.Nombre.Trim(),
            EspecieId = request.EspecieId,
            RazaId = request.RazaId,
            FechaNacimientoAprox = request.FechaNacimientoAprox,
            FotoUrl = request.FotoUrl,
            Dueno = dueno,
            MedicoId = medicoId,
            FechaCreacion = DateTime.Now
        };
        db.Paciente.Add(paciente);

        if (request.TecnicoId is not null)
        {
            db.AsignacionPacienteTecnico.Add(new AsignacionPacienteTecnico
            {
                Paciente = paciente,
                TecnicoId = request.TecnicoId.Value,
                AsignadoPorId = medicoId,
                FechaInicio = DateTime.Now
            });
        }

        actividad.Registrar(medicoId, "PACIENTE_CREADO", $"Registró al paciente {paciente.Nombre}.");
        await db.SaveChangesAsync();

        return await ObtenerAsync(paciente.Id, medicoId, Roles.Medico);
    }

    public async Task<List<PacienteListaDto>> ListarAsync(int usuarioId, string rol)
    {
        var pacientes = await alcance.Pacientes(usuarioId, rol).AsNoTracking()
            .OrderBy(p => p.Nombre)
            .Select(p => new
            {
                p.Id,
                p.Nombre,
                Especie = p.Especie.Nombre,
                Raza = p.Raza.Nombre,
                p.FotoUrl,
                Dueno = p.Dueno.Nombre,
                p.Dueno.Telefono,
                p.Dueno.Direccion,
                p.Dueno.Latitud,
                p.Dueno.Longitud
            })
            .ToListAsync();

        var ids = pacientes.Select(p => p.Id).ToList();


        var tecnicos = await (
            from a in db.AsignacionPacienteTecnico
            join u in db.Usuario on a.TecnicoId equals u.Id
            where a.FechaFin == null && ids.Contains(a.PacienteId)
            select new { a.PacienteId, u.Id, u.Nombre }).ToDictionaryAsync(x => x.PacienteId);


        var proximas = await db.Visita
            .Where(v => v.Estado == EstadosVisita.Pendiente && ids.Contains(v.PacienteId))
            .GroupBy(v => v.PacienteId)
            .Select(g => new { PacienteId = g.Key, Fecha = g.Min(v => v.FechaProgramada) })
            .ToDictionaryAsync(x => x.PacienteId, x => x.Fecha);

        return pacientes.Select(p =>
        {
            tecnicos.TryGetValue(p.Id, out var tecnico);
            DateTime? proxima = proximas.TryGetValue(p.Id, out var fecha) ? fecha : null;

            return new PacienteListaDto(
                p.Id, p.Nombre, p.Especie, p.Raza, p.FotoUrl,
                p.Dueno, p.Telefono, p.Direccion, p.Latitud, p.Longitud,
                tecnico?.Id, tecnico?.Nombre,
                proxima,
                proxima is null ? null : EstadosVisita.Calcular(EstadosVisita.Pendiente, proxima.Value));
        }).ToList();
    }

    /// <summary>Datos del paciente y su historial (registros de visitas en orden cronológico).</summary>
    public async Task<PacienteDetalleDto> ObtenerAsync(int pacienteId, int usuarioId, string rol)
    {
        var p = await alcance.Pacientes(usuarioId, rol).AsNoTracking()
            .Where(x => x.Id == pacienteId)
            .Select(x => new
            {
                x.Id,
                x.Nombre,
                x.EspecieId,
                Especie = x.Especie.Nombre,
                x.RazaId,
                Raza = x.Raza.Nombre,
                x.FechaNacimientoAprox,
                x.FotoUrl,
                Dueno = new DuenoDto(x.Dueno.Id, x.Dueno.Nombre, x.Dueno.Telefono, x.Dueno.Direccion, x.Dueno.Latitud, x.Dueno.Longitud),
                x.MedicoId,
                Medico = x.Medico.Nombre
            })
            .FirstOrDefaultAsync()
            ?? throw ApiException.NoEncontrado("Paciente no encontrado.");

        var tecnico = await (
            from a in db.AsignacionPacienteTecnico
            join u in db.Usuario on a.TecnicoId equals u.Id
            where a.PacienteId == pacienteId && a.FechaFin == null
            select new { u.Id, u.Nombre }).FirstOrDefaultAsync();

        var historial = await (
            from r in db.RegistroVisita
            join v in db.Visita on r.VisitaId equals v.Id
            where v.PacienteId == pacienteId
            orderby r.FechaRegistro
            select new RegistroHistorialDto(
                v.Id, r.FechaRegistro, v.Servicio.Nombre, v.Tecnico.Nombre,
                r.Peso, r.Altura, r.Temperatura, r.CondicionCorporal, r.Observaciones, r.FotoUrl))
            .ToListAsync();

        return new PacienteDetalleDto(
            p.Id, p.Nombre, p.EspecieId, p.Especie, p.RazaId, p.Raza, p.FechaNacimientoAprox, p.FotoUrl,
            p.Dueno, p.MedicoId, p.Medico, tecnico?.Id, tecnico?.Nombre, historial);
    }

   
    public async Task<AsignacionPacienteResultadoDto> AsignarTecnicoAsync(int pacienteId, int tecnicoId, int medicoId)
    {
        var paciente = await db.Paciente.FirstOrDefaultAsync(p => p.Id == pacienteId && p.MedicoId == medicoId)
                       ?? throw ApiException.NoEncontrado("Paciente no encontrado.");

        await ValidarTecnicoDelMedicoAsync(tecnicoId, medicoId);

        var actual = await db.AsignacionPacienteTecnico
            .FirstOrDefaultAsync(a => a.PacienteId == pacienteId && a.FechaFin == null);

        if (actual?.TecnicoId == tecnicoId)
            throw ApiException.Conflicto("El paciente ya está asignado a ese técnico.");

        var ahora = DateTime.Now;
        await using var transaccion = await db.Database.BeginTransactionAsync();

        if (actual is not null)
        {
            actual.FechaFin = ahora;
            // Se guarda el cierre antes de crear la nueva (índice único de asignación vigente)
            await db.SaveChangesAsync();
        }

        db.AsignacionPacienteTecnico.Add(new AsignacionPacienteTecnico
        {
            PacienteId = pacienteId,
            TecnicoId = tecnicoId,
            AsignadoPorId = medicoId,
            FechaInicio = ahora
        });

        var visitasTransferidas = await db.Visita
            .Where(v => v.PacienteId == pacienteId && v.Estado == EstadosVisita.Pendiente)
            .ExecuteUpdateAsync(s => s.SetProperty(v => v.TecnicoId, tecnicoId));

        actividad.Registrar(medicoId, "PACIENTE_ASIGNADO", $"Asignó al paciente {paciente.Nombre} a un técnico.");
        await db.SaveChangesAsync();
        await transaccion.CommitAsync();

        return new AsignacionPacienteResultadoDto("Técnico asignado correctamente.", visitasTransferidas);
    }

    private async Task ValidarTecnicoDelMedicoAsync(int tecnicoId, int medicoId)
    {
        var esSuyo = await alcance.TecnicosDe(medicoId).ContainsAsync(tecnicoId);
        var activo = await db.Usuario.AnyAsync(u => u.Id == tecnicoId && u.Activo);

        if (!esSuyo || !activo)
            throw ApiException.BadRequest("El técnico no está a su cargo o está desactivado.");
    }
}
