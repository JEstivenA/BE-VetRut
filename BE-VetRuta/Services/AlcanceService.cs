using BE_VetRuta.Utilitys;
using BE_VetRuta.Data;
using BE_VetRuta.Entities;

namespace BE_VetRuta.Services;

public class AlcanceService(VetRutaDbContext db)
{
    public IQueryable<Paciente> Pacientes(int usuarioId, string rol) => rol switch
    {
        Roles.Director => db.Paciente,
        Roles.Medico => db.Paciente.Where(p => p.MedicoId == usuarioId),
        Roles.Tecnico => db.Paciente.Where(p => db.AsignacionPacienteTecnico
            .Any(a => a.PacienteId == p.Id && a.TecnicoId == usuarioId && a.FechaFin == null)),
        _ => db.Paciente.Where(p => false)
    };

    public IQueryable<Visita> Visitas(int usuarioId, string rol) => rol switch
    {
        Roles.Director => db.Visita,
        Roles.Medico => db.Visita.Where(v => v.Paciente.MedicoId == usuarioId),
        Roles.Tecnico => db.Visita.Where(v => v.TecnicoId == usuarioId),
        _ => db.Visita.Where(v => false)
    };

    public IQueryable<int> TecnicosDe(int medicoId) =>
        db.AsignacionTecnicoMedico
            .Where(a => a.MedicoId == medicoId && a.FechaFin == null)
            .Select(a => a.TecnicoId);
}
