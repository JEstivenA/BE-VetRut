using System;
using System.Collections.Generic;

namespace BE_VetRuta.Entities;

public partial class Servicio
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public int DuracionMinutos { get; set; }

    public virtual ICollection<Visita> Visita { get; set; } = new List<Visita>();
}
