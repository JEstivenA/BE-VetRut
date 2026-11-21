using System.ComponentModel.DataAnnotations;

namespace BE_VetRuta.DTOs
{
    public record LoginRequest(
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        string Email,

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        string Password
    );

    public record RefreshRequest(
        [Required(ErrorMessage = "El refresh token es obligatorio.")]
        string RefreshToken
    );

    public record UsuarioSesionDto(int Id, string Nombre, string Email, string Rol);

    public record LoginResponse(
        string AccessToken,
        DateTime ExpiraEn,
        string RefreshToken,
        UsuarioSesionDto Usuario
    );
}
