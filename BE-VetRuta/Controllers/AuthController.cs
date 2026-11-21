using BE_VetRuta.DTOs;
using BE_VetRuta.Services;
using BE_VetRuta.Utilitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE_VetRuta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request) =>
        Ok(await authService.LoginAsync(request));

        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponse>> Refresh(RefreshRequest request) =>
            Ok(await authService.RefreshAsync(request.RefreshToken));

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshRequest request)
        {
            await authService.LogoutAsync(request.RefreshToken, User.GetUsuarioId());
            return NoContent();
        }

        /// <summary>TEST: Datos del usuario autenticado.</summary>
        [Authorize]
        [HttpGet("me")]
        public ActionResult<UsuarioSesionDto> Me() =>
            Ok(new UsuarioSesionDto(
                User.GetUsuarioId(),
                User.FindFirst("name")!.Value,
                User.FindFirst("email")!.Value,
                User.GetRol()));
    }
}
