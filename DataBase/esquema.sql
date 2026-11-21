/* =============================================================================
   VetRuta - 01. Creación de la base de datos y tablas
   SQL Server 2019+
   ============================================================================= */
IF DB_ID(N'VetRuta') IS NULL
    CREATE DATABASE VetRuta;
GO
USE VetRuta;
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ---------- Catálogos ---------- */
CREATE TABLE Rol (
    Id      INT PRIMARY KEY,              -- 1 ROOT, 2 DIRECTOR, 3 MEDICO, 4 TECNICO
    Nombre  VARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE Zona (
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Nombre  NVARCHAR(80) NOT NULL UNIQUE
);

CREATE TABLE Especie (
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Nombre  NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Raza (
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    EspecieId  INT NOT NULL REFERENCES Especie(Id),
    Nombre     NVARCHAR(80) NOT NULL
);

CREATE TABLE Servicio (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    Nombre           NVARCHAR(100) NOT NULL UNIQUE,
    Precio           DECIMAL(10,2) NOT NULL,
    DuracionMinutos  INT NOT NULL
);
GO

/* ---------- Usuarios y sesión ---------- */
CREATE TABLE Usuario (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    Nombre        NVARCHAR(150) NOT NULL,
    Email         NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash  NVARCHAR(100) NOT NULL,
    RolId         INT NOT NULL REFERENCES Rol(Id),
    ZonaId        INT NULL REFERENCES Zona(Id),    -- zona en la que trabaja el técnico
    Activo        BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE()
);

CREATE TABLE RefreshToken (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId        INT NOT NULL REFERENCES Usuario(Id),
    Token            NVARCHAR(200) NOT NULL UNIQUE,
    FechaExpiracion  DATETIME2 NOT NULL,
    Revocado         BIT NOT NULL DEFAULT 0,
    FechaCreacion    DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO


CREATE TABLE AsignacionTecnicoMedico (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    TecnicoId      INT NOT NULL REFERENCES Usuario(Id),
    MedicoId       INT NOT NULL REFERENCES Usuario(Id),
    AsignadoPorId  INT NOT NULL REFERENCES Usuario(Id),   -- director
    FechaInicio    DATETIME2 NOT NULL DEFAULT GETDATE(),
    FechaFin       DATETIME2 NULL
);

-- Un técnico pertenece a un solo médico a la vez
CREATE UNIQUE INDEX UX_AsignacionTecnicoMedico_Vigente
    ON AsignacionTecnicoMedico (TecnicoId) WHERE FechaFin IS NULL;
GO

/* ---------- Pacientes ---------- */
CREATE TABLE Dueno (
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    Nombre     NVARCHAR(150) NOT NULL,
    Telefono   VARCHAR(20)   NOT NULL,
    Direccion  NVARCHAR(300) NOT NULL,
    Latitud    DECIMAL(9,6)  NOT NULL,   -- ubicación para el mapa
    Longitud   DECIMAL(9,6)  NOT NULL
);

CREATE TABLE Paciente (
    Id                    INT IDENTITY(1,1) PRIMARY KEY,
    Nombre                NVARCHAR(80)  NOT NULL,
    EspecieId             INT NOT NULL REFERENCES Especie(Id),
    RazaId                INT NOT NULL REFERENCES Raza(Id),
    FechaNacimientoAprox  DATE NOT NULL,
    FotoUrl               NVARCHAR(500) NULL,
    DuenoId               INT NOT NULL REFERENCES Dueno(Id),
    MedicoId              INT NOT NULL REFERENCES Usuario(Id),   -- médico que lo creó
    FechaCreacion         DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO



CREATE TABLE AsignacionPacienteTecnico (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    PacienteId     INT NOT NULL REFERENCES Paciente(Id),
    TecnicoId      INT NOT NULL REFERENCES Usuario(Id),
    AsignadoPorId  INT NOT NULL REFERENCES Usuario(Id),   -- médico
    FechaInicio    DATETIME2 NOT NULL DEFAULT GETDATE(),
    FechaFin       DATETIME2 NULL
);


CREATE UNIQUE INDEX UX_AsignacionPacienteTecnico_Vigente
    ON AsignacionPacienteTecnico (PacienteId) WHERE FechaFin IS NULL;
GO


CREATE TABLE Visita (
    Id                 INT IDENTITY(1,1) PRIMARY KEY,
    PacienteId         INT NOT NULL REFERENCES Paciente(Id),
    ServicioId         INT NOT NULL REFERENCES Servicio(Id),
    TecnicoId          INT NOT NULL REFERENCES Usuario(Id),
    FechaProgramada    DATETIME2 NOT NULL,
    Estado             VARCHAR(12) NOT NULL DEFAULT 'PENDIENTE'
                       CHECK (Estado IN ('PENDIENTE','REALIZADA','CANCELADA')),
    VisitaOrigenId     INT NULL REFERENCES Visita(Id),        -- visita que se reprogramó
    MotivoCancelacion  NVARCHAR(300) NULL,
    CreadaPorId        INT NOT NULL REFERENCES Usuario(Id),
    FechaCreacion      DATETIME2 NOT NULL DEFAULT GETDATE()
);



CREATE TABLE RegistroVisita (
    Id                 INT IDENTITY(1,1) PRIMARY KEY,
    VisitaId           INT NOT NULL UNIQUE REFERENCES Visita(Id),
    Peso               DECIMAL(7,3)   NOT NULL,   -- kg
    Altura             DECIMAL(5,1)   NOT NULL,   -- cm
    Temperatura        DECIMAL(4,1)   NOT NULL,   -- °C
    CondicionCorporal  INT            NOT NULL CHECK (CondicionCorporal BETWEEN 1 AND 9),
    Observaciones      NVARCHAR(1000) NULL,
    Latitud            DECIMAL(9,6)   NOT NULL, 
    Longitud           DECIMAL(9,6)   NOT NULL,
    FotoUrl            NVARCHAR(500)  NOT NULL,   -- foto de la visita
    FechaRegistro      DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE Actividad (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId    INT NOT NULL REFERENCES Usuario(Id),
    Accion       VARCHAR(50)   NOT NULL,   -- ej. VISITA_REGISTRADA
    Descripcion  NVARCHAR(300) NOT NULL,
    Fecha        DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO