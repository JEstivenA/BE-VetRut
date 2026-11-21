using System;
using System.Collections.Generic;

namespace BE_VetRuta.Entities;

public partial class Paciente
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int EspecieId { get; set; }

    public int RazaId { get; set; }

    public DateOnly FechaNacimientoAprox { get; set; }

    public string? FotoUrl { get; set; }

    public int DuenoId { get; set; }

    public int MedicoId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual AsignacionPacienteTecnico? AsignacionPacienteTecnico { get; set; }

    public virtual Dueno Dueno { get; set; } = null!;

    public virtual Especie Especie { get; set; } = null!;

    public virtual Usuario Medico { get; set; } = null!;

    public virtual Raza Raza { get; set; } = null!;

    public virtual ICollection<Visita> Visita { get; set; } = new List<Visita>();
}
