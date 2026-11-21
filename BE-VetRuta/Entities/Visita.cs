using System;
using System.Collections.Generic;

namespace BE_VetRuta.Entities;

public partial class Visita
{
    public int Id { get; set; }

    public int PacienteId { get; set; }

    public int ServicioId { get; set; }

    public int TecnicoId { get; set; }

    public DateTime FechaProgramada { get; set; }

    public string Estado { get; set; } = null!;

    public int? VisitaOrigenId { get; set; }

    public string? MotivoCancelacion { get; set; }

    public int CreadaPorId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Usuario CreadaPor { get; set; } = null!;

    public virtual ICollection<Visita> InverseVisitaOrigen { get; set; } = new List<Visita>();

    public virtual Paciente Paciente { get; set; } = null!;

    public virtual RegistroVisita? RegistroVisita { get; set; }

    public virtual Servicio Servicio { get; set; } = null!;

    public virtual Usuario Tecnico { get; set; } = null!;

    public virtual Visita? VisitaOrigen { get; set; }
}
