namespace BE_VetRuta.DTOs
{
    public record ZonaDto(int Id, string Nombre);

    public record RazaDto(int Id, string Nombre);

    public record EspecieDto(int Id, string Nombre, List<RazaDto> Razas);

    public record ServicioDto(int Id, string Nombre, decimal Precio, int DuracionMinutos);
}
