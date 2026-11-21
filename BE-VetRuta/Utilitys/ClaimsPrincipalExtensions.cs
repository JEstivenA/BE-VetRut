using System.Security.Claims;

namespace BE_VetRuta.Utilitys
{
    /// <summary>Lee los datos del usuario autenticado desde el token.</summary>
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUsuarioId(this ClaimsPrincipal user) =>
            int.Parse(user.FindFirstValue("sub")!);

        public static string GetRol(this ClaimsPrincipal user) =>
            user.FindFirstValue("role")!;
    }
}
