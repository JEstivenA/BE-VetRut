using System;
using System.Collections.Generic;

namespace BE_VetRuta.Entities;

public partial class Dueno
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public decimal Latitud { get; set; }

    public decimal Longitud { get; set; }

    public virtual ICollection<Paciente> Paciente { get; set; } = new List<Paciente>();
}
