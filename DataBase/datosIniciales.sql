/* =============================================================================
   VetRuta - 02. Catálogos y usuarios iniciales

   Usuarios (contraseña para todos: VetRuta2026!):
     root@vetruta.com       ROOT
     director@vetruta.com   DIRECTOR
     medico@vetruta.com     MEDICO
     tecnico@vetruta.com    TECNICO (a cargo de medico@vetruta.com)
   ============================================================================= */
USE VetRuta;
GO

INSERT INTO Rol (Id, Nombre) VALUES (1, 'ROOT'), (2, 'DIRECTOR'), (3, 'MEDICO'), (4, 'TECNICO');

INSERT INTO Zona (Nombre) VALUES
 (N'Zona 1'), (N'Zona 4'), (N'Zona 9'), (N'Zona 10'), (N'Zona 11'), (N'Zona 14'), (N'Zona 15'), (N'Zona 16'), (N'Mixco'), (N'Villa Nueva');

INSERT INTO Especie (Nombre) VALUES (N'Perro'), (N'Gato'), (N'Ave'), (N'Conejo'), (N'Reptil');

INSERT INTO Raza (EspecieId, Nombre)
SELECT e.Id, v.Raza
FROM (VALUES
  (N'Perro', N'Mestizo'), (N'Perro', N'Labrador'), (N'Perro', N'Pastor Alemán'), (N'Perro', N'Chihuahua'), (N'Perro', N'Poodle'),
  (N'Gato', N'Mestizo'), (N'Gato', N'Siamés'), (N'Gato', N'Persa'),
  (N'Ave', N'Perico australiano'), (N'Ave', N'Canario'), (N'Ave', N'Cotorra'),
  (N'Conejo', N'Mestizo'), (N'Conejo', N'Holland Lop'), (N'Conejo', N'Cabeza de León'),
  (N'Reptil', N'Iguana'), (N'Reptil', N'Tortuga'), (N'Reptil', N'Gecko')
) v (Especie, Raza)
JOIN Especie e ON e.Nombre = v.Especie;

INSERT INTO Servicio (Nombre, Precio, DuracionMinutos) VALUES
 (N'Control de peso',         75.00, 20),
 (N'Vacunación',             150.00, 30),
 (N'Desparasitación',        100.00, 20),
 (N'Curación de heridas',    175.00, 45),
 (N'Control postoperatorio', 200.00, 40);

-- Hash BCrypt de "VetRuta2026!"
DECLARE @Hash NVARCHAR(100) = N'$2b$11$S4.VKbRdFLNZi2dJ8njeo..D5YdYyKf2A2YVvxMBLq0Wk5rFWMu4S';

INSERT INTO Usuario (Nombre, Email, PasswordHash, RolId, ZonaId) VALUES
 (N'Administrador', N'root@vetruta.com',     @Hash, 1, NULL),
 (N'Ana López',     N'director@vetruta.com', @Hash, 2, NULL),
 (N'Carlos Méndez', N'medico@vetruta.com',   @Hash, 3, NULL),
 (N'José Pérez',    N'tecnico@vetruta.com',  @Hash, 4, (SELECT Id FROM Zona WHERE Nombre = N'Zona 10'));

INSERT INTO AsignacionTecnicoMedico (TecnicoId, MedicoId, AsignadoPorId)
SELECT (SELECT Id FROM Usuario WHERE Email = N'tecnico@vetruta.com'),
       (SELECT Id FROM Usuario WHERE Email = N'medico@vetruta.com'),
       (SELECT Id FROM Usuario WHERE Email = N'director@vetruta.com');
GO