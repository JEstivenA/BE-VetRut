namespace BE_VetRuta.Utilitys
{
    public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ApiException ex)
            {
                await EscribirAsync(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error no controlado");
                await EscribirAsync(context, StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado.");
            }
        }

        private static Task EscribirAsync(HttpContext context, int statusCode, string mensaje)
        {
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync(new { mensaje });
        }
    }
}
