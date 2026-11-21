using System.ComponentModel.DataAnnotations;

namespace BE_VetRuta.DTOs
{
    public record TecnicoDto(int Id, string Nombre, string? Zona, bool Activo);

    public record MedicoConTecnicosDto(int Id, string Nombre, List<TecnicoDto> Tecnicos);

    public record JerarquiaDto(List<MedicoConTecnicosDto> Medicos, List<TecnicoDto> TecnicosSinMedico);

    public record AsignarTecnicoRequest(
        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar el médico.")]
        int MedicoId
    );

    public record AsignacionResultadoDto(string Mensaje, int PacientesLiberados, int VisitasCanceladas);
}
