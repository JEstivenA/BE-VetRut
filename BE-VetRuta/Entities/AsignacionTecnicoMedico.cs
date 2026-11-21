using System;
using System.Collections.Generic;

namespace BE_VetRuta.Entities;

public partial class AsignacionTecnicoMedico
{
    public int Id { get; set; }

    public int TecnicoId { get; set; }

    public int MedicoId { get; set; }

    public int AsignadoPorId { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public virtual Usuario AsignadoPor { get; set; } = null!;

    public virtual Usuario Medico { get; set; } = null!;

    public virtual Usuario Tecnico { get; set; } = null!;
}
