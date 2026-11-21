using System.ComponentModel.DataAnnotations;

namespace BE_VetRuta.DTOs
{
    public record UsuarioDto(
        int Id,
        string Nombre,
        string Email,
        string Rol,
        int? ZonaId,
        string? Zona,
        bool Activo
    );

    public record CrearUsuarioRequest(
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(150)]
        string Nombre,

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        string Email,

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        string Password,

        [Required(ErrorMessage = "El rol es obligatorio.")]
        string Rol,

        int? ZonaId
    );

    public record CambiarEstadoRequest(bool Activo);
}
