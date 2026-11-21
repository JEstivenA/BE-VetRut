using BE_VetRuta.Data;
using BE_VetRuta.DTOs;
using BE_VetRuta.Entities;
using BE_VetRuta.Utilitys;
using Microsoft.EntityFrameworkCore;

namespace BE_VetRuta.Services
{
    public class JerarquiaService(VetRutaDbContext db, ActividadService actividad)
    {
        public async Task<JerarquiaDto> ObtenerAsync()
        {
            var medicos = await db.Usuario.AsNoTracking()
                .Where(u => u.Rol.Nombre == Roles.Medico && u.Activo)
                .OrderBy(u => u.Nombre)
                .Select(u => new { u.Id, u.Nombre })
                .ToListAsync();

            var tecnicos = await (
                from t in db.Usuario.AsNoTracking()
                where t.Rol.Nombre == Roles.Tecnico
                join a in db.AsignacionTecnicoMedico.Where(a => a.FechaFin == null)
                    on t.Id equals a.TecnicoId into asignaciones
                from a in asignaciones.DefaultIfEmpty()
                orderby t.Nombre
                select new
                {
                    t.Id,
                    t.Nombre,
                    Zona = t.Zona != null ? t.Zona.Nombre : null,
                    t.Activo,
                    MedicoId = (int?)a.MedicoId
                }).ToListAsync();

            var idsMedicos = medicos.Select(m => m.Id).ToHashSet();

            var lista = medicos
                .Select(m => new MedicoConTecnicosDto(
                    m.Id,
                    m.Nombre,
                    tecnicos.Where(t => t.MedicoId == m.Id)
                            .Select(t => new TecnicoDto(t.Id, t.Nombre, t.Zona, t.Activo))
                            .ToList()))
                .ToList();

            var sinMedico = tecnicos
                .Where(t => t.MedicoId is null || !idsMedicos.Contains(t.MedicoId.Value))
                .Select(t => new TecnicoDto(t.Id, t.Nombre, t.Zona, t.Activo))
                .ToList();

            return new JerarquiaDto(lista, sinMedico);
        }

        public async Task<AsignacionResultadoDto> AsignarTecnicoAsync(int tecnicoId, int medicoId, int directorId)
        {
            var tecnico = await db.Usuario.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Id == tecnicoId);

            if (tecnico is null || tecnico.Rol.Nombre != Roles.Tecnico)
                throw ApiException.NoEncontrado("Técnico no encontrado.");
            if (!tecnico.Activo)
                throw ApiException.BadRequest("El técnico está desactivado.");

            var medico = await db.Usuario.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Id == medicoId);
            if (medico is null || medico.Rol.Nombre != Roles.Medico)
                throw ApiException.NoEncontrado("Médico no encontrado.");
            if (!medico.Activo)
                throw ApiException.BadRequest("El médico está desactivado.");

            var actual = await db.AsignacionTecnicoMedico
                .FirstOrDefaultAsync(a => a.TecnicoId == tecnicoId && a.FechaFin == null);

            if (actual?.MedicoId == medicoId)
                throw ApiException.Conflicto("El técnico ya está asignado a ese médico.");

            var ahora = DateTime.Now;
            int pacientesLiberados = 0, visitasCanceladas = 0;

            await using var transaccion = await db.Database.BeginTransactionAsync();

            if (actual is not null)
            {
                actual.FechaFin = ahora;

                pacientesLiberados = await db.AsignacionPacienteTecnico
                    .Where(a => a.TecnicoId == tecnicoId && a.FechaFin == null)
                    .ExecuteUpdateAsync(s => s.SetProperty(a => a.FechaFin, (DateTime?)ahora));

                visitasCanceladas = await db.Visita
                    .Where(v => v.TecnicoId == tecnicoId && v.Estado == EstadosVisita.Pendiente)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(v => v.Estado, EstadosVisita.Cancelada)
                        .SetProperty(v => v.MotivoCancelacion, "El técnico fue reasignado a otro médico."));

                await db.SaveChangesAsync();
            }

            db.AsignacionTecnicoMedico.Add(new AsignacionTecnicoMedico
            {
                TecnicoId = tecnicoId,
                MedicoId = medicoId,
                AsignadoPorId = directorId,
                FechaInicio = ahora
            });
            actividad.Registrar(directorId, "TECNICO_ASIGNADO", $"Asignó al técnico {tecnico.Nombre} al médico {medico.Nombre}.");
            await db.SaveChangesAsync();

            await transaccion.CommitAsync();

            return new AsignacionResultadoDto(
                $"{tecnico.Nombre} quedó asignado a {medico.Nombre}.",
                pacientesLiberados,
                visitasCanceladas);
        }
    }

}
