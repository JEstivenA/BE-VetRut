using BE_VetRuta.Data;
using BE_VetRuta.Entities;

namespace BE_VetRuta.Services
{
    public class ActividadService(VetRutaDbContext db)
    {
        /// <summary>
        /// Agrega la actividad al contexto. Se guarda en el mismo SaveChanges de la
        /// operación, así la acción y su registro quedan juntos o no queda ninguno.
        /// </summary>
        public void Registrar(int usuarioId, string accion, string descripcion) =>
            db.Actividad.Add(new Actividad
            {
                UsuarioId = usuarioId,
                Accion = accion,
                Descripcion = descripcion,
                Fecha = DateTime.Now
            });
    }
}
