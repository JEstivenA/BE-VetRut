using System;
using System.Collections.Generic;

namespace BE_VetRuta.Entities;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RolId { get; set; }

    public int? ZonaId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<Actividad> Actividad { get; set; } = new List<Actividad>();

    public virtual ICollection<AsignacionPacienteTecnico> AsignacionPacienteTecnicoAsignadoPor { get; set; } = new List<AsignacionPacienteTecnico>();

    public virtual ICollection<AsignacionPacienteTecnico> AsignacionPacienteTecnicoTecnico { get; set; } = new List<AsignacionPacienteTecnico>();

    public virtual ICollection<AsignacionTecnicoMedico> AsignacionTecnicoMedicoAsignadoPor { get; set; } = new List<AsignacionTecnicoMedico>();

    public virtual ICollection<AsignacionTecnicoMedico> AsignacionTecnicoMedicoMedico { get; set; } = new List<AsignacionTecnicoMedico>();

    public virtual AsignacionTecnicoMedico? AsignacionTecnicoMedicoTecnico { get; set; }

    public virtual ICollection<Paciente> Paciente { get; set; } = new List<Paciente>();

    public virtual ICollection<RefreshToken> RefreshToken { get; set; } = new List<RefreshToken>();

    public virtual Rol Rol { get; set; } = null!;

    public virtual ICollection<Visita> VisitaCreadaPor { get; set; } = new List<Visita>();

    public virtual ICollection<Visita> VisitaTecnico { get; set; } = new List<Visita>();

    public virtual Zona? Zona { get; set; }
}
