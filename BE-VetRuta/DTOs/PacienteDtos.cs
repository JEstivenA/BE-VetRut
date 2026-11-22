using System.ComponentModel.DataAnnotations;

namespace BE_VetRuta.DTOs;

public record DuenoRequest(
    [Required(ErrorMessage = "El nombre del dueño es obligatorio.")]
    [MaxLength(150)]
    string Nombre,

    [Required(ErrorMessage = "El teléfono del dueño es obligatorio.")]
    [MaxLength(20)]
    string Telefono,

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [MaxLength(300)]
    string Direccion,

    [Range(-90, 90, ErrorMessage = "Latitud no válida.")]
    decimal Latitud,

    [Range(-180, 180, ErrorMessage = "Longitud no válida.")]
    decimal Longitud
);

public record CrearPacienteRequest(
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(80)]
    string Nombre,

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar la especie.")]
    int EspecieId,

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar la raza.")]
    int RazaId,

    DateOnly FechaNacimientoAprox,

    string? FotoUrl,

    [Required(ErrorMessage = "Los datos del dueño son obligatorios.")]
    DuenoRequest Dueno,

    // Opcional: asignar técnico al crear
    int? TecnicoId
);

public record AsignarTecnicoPacienteRequest(
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar el técnico.")]
    int TecnicoId
);

public record DuenoDto(int Id, string Nombre, string Telefono, string Direccion, decimal Latitud, decimal Longitud);


public record PacienteListaDto(
    int Id,
    string Nombre,
    string Especie,
    string Raza,
    string? FotoUrl,
    string Dueno,
    string Telefono,
    string Direccion,
    decimal Latitud,
    decimal Longitud,
    int? TecnicoId,
    string? Tecnico,
    DateTime? ProximaVisita,
    string? EstadoProximaVisita);

public record RegistroHistorialDto(
    int VisitaId,
    DateTime FechaRegistro,
    string Servicio,
    string Tecnico,
    decimal Peso,
    decimal Altura,
    decimal Temperatura,
    int CondicionCorporal,
    string? Observaciones,
    string FotoUrl);

public record PacienteDetalleDto(
    int Id,
    string Nombre,
    int EspecieId,
    string Especie,
    int RazaId,
    string Raza,
    DateOnly FechaNacimientoAprox,
    string? FotoUrl,
    DuenoDto Dueno,
    int MedicoId,
    string Medico,
    int? TecnicoId,
    string? Tecnico,
    List<RegistroHistorialDto> Historial);

public record AsignacionPacienteResultadoDto(string Mensaje, int VisitasTransferidas);
