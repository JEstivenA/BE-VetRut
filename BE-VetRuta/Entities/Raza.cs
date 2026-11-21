using System;
using System.Collections.Generic;

namespace BE_VetRuta.Entities;

public partial class Raza
{
    public int Id { get; set; }

    public int EspecieId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual Especie Especie { get; set; } = null!;

    public virtual ICollection<Paciente> Paciente { get; set; } = new List<Paciente>();
}
