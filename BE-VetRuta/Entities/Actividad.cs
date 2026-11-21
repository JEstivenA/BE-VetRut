using System;
using System.Collections.Generic;

namespace BE_VetRuta.Entities;

public partial class Actividad
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string Accion { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
