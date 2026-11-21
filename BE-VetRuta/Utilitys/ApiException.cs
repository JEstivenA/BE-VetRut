namespace BE_VetRuta.Utilitys
{
    public class ApiException(int statusCode, string mensaje) : Exception(mensaje)
    {
        public int StatusCode { get; } = statusCode;

        public static ApiException BadRequest(string mensaje) => new(StatusCodes.Status400BadRequest, mensaje);
        public static ApiException NoAutorizado(string mensaje) => new(StatusCodes.Status401Unauthorized, mensaje);
        public static ApiException Prohibido(string mensaje) => new(StatusCodes.Status403Forbidden, mensaje);
        public static ApiException NoEncontrado(string mensaje) => new(StatusCodes.Status404NotFound, mensaje);
        public static ApiException Conflicto(string mensaje) => new(StatusCodes.Status409Conflict, mensaje);
    }
}
