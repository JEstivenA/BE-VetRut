using System;
using System.Collections.Generic;

namespace BE_VetRuta.Entities;

public partial class RegistroVisita
{
    public int Id { get; set; }

    public int VisitaId { get; set; }

    public decimal Peso { get; set; }

    public decimal Altura { get; set; }

    public decimal Temperatura { get; set; }

    public int CondicionCorporal { get; set; }

    public string? Observaciones { get; set; }

    public decimal Latitud { get; set; }

    public decimal Longitud { get; set; }

    public string FotoUrl { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public virtual Visita Visita { get; set; } = null!;
}
