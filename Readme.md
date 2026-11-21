### Decisiones de diseño en BD

- **Roles:** tabla Rol (ROOT, DIRECTOR, MEDICO, TECNICO); cada usuario tiene un rol.
- **Usuarios:** no se eliminan; root los activa o desactiva con el campo Activo.
- **Jerarquía con historial:** las asignaciones técnico→médico y paciente→técnico se guardan con `FechaInicio` / `FechaFin`. La vigente es la que tiene `FechaFin` en NULL. Reasignar cierra la vigente y crea una nueva. Un índice único filtrado garantiza un solo médico por técnico y un solo técnico por paciente a la vez.
- **Zonas:** cada técnico trabaja en una zona; un médico cubre varias zonas a través de sus técnicos.
- **Ubicación:** la dirección y las coordenadas se guardan en Dueno y se usan en el mapa.
- **Visitas:** se separa lo programado (Visita) de lo capturado por el técnico (RegistroVisita, uno por visita).
- **Estados:** se guardan PENDIENTE, REALIZADA y CANCELADA. _Atrasada_ se calcula: es una visita pendiente con la fecha vencida. Una visita atrasada todavía puede registrarse.
- **Historial inmutable:** un registro de visita no se edita ni se borra. Para reprogramar, se cancela la visita (con motivo) y se crea una nueva con VisitaOrigenId, así la fecha original queda en el historial.
- **Actividades:** tabla Actividad (bitácora) donde la API registra cada acción. Cada rol la consulta según su alcance.
- **Sesión:** JWT más refresh token guardado en la tabla RefreshToken.
- **Fotos:** se guardan en el servidor; la base solo guarda la ruta.
