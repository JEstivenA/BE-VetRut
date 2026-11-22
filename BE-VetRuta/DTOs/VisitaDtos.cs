using System.ComponentModel.DataAnnotations;

namespace VetRuta.Api.DTOs;

public record CrearVisitaRequest(
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar el paciente.")]
    int PacienteId,

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar el servicio.")]
    int ServicioId,

    DateTime FechaProgramada
);

public record CancelarVisitaRequest(
    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [MaxLength(250)]
    string Motivo
);

public record ReprogramarVisitaRequest(
    DateTime NuevaFecha,

    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [MaxLength(250)]
    string Motivo
);

public record RegistrarVisitaRequest(
    [Range(0.001, 200, ErrorMessage = "El peso debe estar entre 0.001 y 200 kg.")]
    decimal Peso,

    [Range(0.1, 300, ErrorMessage = "La altura debe estar entre 0.1 y 300 cm.")]
    decimal Altura,

    [Range(15, 45, ErrorMessage = "La temperatura debe estar entre 15 y 45 °C.")]
    decimal Temperatura,

    [Range(1, 9, ErrorMessage = "La condición corporal debe estar entre 1 y 9.")]
    int CondicionCorporal,

    [MaxLength(1000)]
    string? Observaciones,

    [Range(-90, 90, ErrorMessage = "Latitud no válida.")]
    decimal Latitud,

    [Range(-180, 180, ErrorMessage = "Longitud no válida.")]
    decimal Longitud,

    [Required(ErrorMessage = "La foto de la visita es obligatoria.")]
    string FotoUrl
);

public record VisitaDto(
    int Id,
    int PacienteId,
    string Paciente,
    string Especie,
    int ServicioId,
    string Servicio,
    int TecnicoId,
    string Tecnico,
    DateTime FechaProgramada,
    string Estado,
    string? MotivoCancelacion,
    int? VisitaOrigenId,
    string Direccion,
    decimal Latitud,
    decimal Longitud);

public record RegistroVisitaDto(
    decimal Peso,
    decimal Altura,
    decimal Temperatura,
    int CondicionCorporal,
    string? Observaciones,
    decimal Latitud,
    decimal Longitud,
    string FotoUrl,
    DateTime FechaRegistro);

public record VisitaDetalleDto(VisitaDto Visita, RegistroVisitaDto? Registro);
