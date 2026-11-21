using System;
using System.Collections.Generic;

namespace BE_VetRuta.Entities;

public partial class Especie
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Paciente> Paciente { get; set; } = new List<Paciente>();

    public virtual ICollection<Raza> Raza { get; set; } = new List<Raza>();
}
