using System;
using System.Collections.Generic;
using BE_VetRuta.Entities;
using Microsoft.EntityFrameworkCore;

namespace BE_VetRuta.Data;

public partial class VetRutaDbContext : DbContext
{
    public VetRutaDbContext(DbContextOptions<VetRutaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actividad> Actividad { get; set; }

    public virtual DbSet<AsignacionPacienteTecnico> AsignacionPacienteTecnico { get; set; }

    public virtual DbSet<AsignacionTecnicoMedico> AsignacionTecnicoMedico { get; set; }

    public virtual DbSet<Dueno> Dueno { get; set; }

    public virtual DbSet<Especie> Especie { get; set; }

    public virtual DbSet<Paciente> Paciente { get; set; }

    public virtual DbSet<Raza> Raza { get; set; }

    public virtual DbSet<RefreshToken> RefreshToken { get; set; }

    public virtual DbSet<RegistroVisita> RegistroVisita { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<Servicio> Servicio { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    public virtual DbSet<Visita> Visita { get; set; }

    public virtual DbSet<Zona> Zona { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actividad>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Activida__3214EC07C11B8060");

            entity.Property(e => e.Accion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion).HasMaxLength(300);
            entity.Property(e => e.Fecha).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Actividad)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Actividad__Usuar__75A278F5");
        });

        modelBuilder.Entity<AsignacionPacienteTecnico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Asignaci__3214EC07D92F0FF2");

            entity.HasIndex(e => e.PacienteId, "UX_AsignacionPacienteTecnico_Vigente")
                .IsUnique()
                .HasFilter("([FechaFin] IS NULL)");

            entity.Property(e => e.FechaInicio).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.AsignadoPor).WithMany(p => p.AsignacionPacienteTecnicoAsignadoPor)
                .HasForeignKey(d => d.AsignadoPorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Asignacio__Asign__628FA481");

            entity.HasOne(d => d.Paciente).WithOne(p => p.AsignacionPacienteTecnico)
                .HasForeignKey<AsignacionPacienteTecnico>(d => d.PacienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Asignacio__Pacie__60A75C0F");

            entity.HasOne(d => d.Tecnico).WithMany(p => p.AsignacionPacienteTecnicoTecnico)
                .HasForeignKey(d => d.TecnicoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Asignacio__Tecni__619B8048");
        });

        modelBuilder.Entity<AsignacionTecnicoMedico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Asignaci__3214EC0703D54DED");

            entity.HasIndex(e => e.TecnicoId, "UX_AsignacionTecnicoMedico_Vigente")
                .IsUnique()
                .HasFilter("([FechaFin] IS NULL)");

            entity.Property(e => e.FechaInicio).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.AsignadoPor).WithMany(p => p.AsignacionTecnicoMedicoAsignadoPor)
                .HasForeignKey(d => d.AsignadoPorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Asignacio__Asign__5441852A");

            entity.HasOne(d => d.Medico).WithMany(p => p.AsignacionTecnicoMedicoMedico)
                .HasForeignKey(d => d.MedicoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Asignacio__Medic__534D60F1");

            entity.HasOne(d => d.Tecnico).WithOne(p => p.AsignacionTecnicoMedicoTecnico)
                .HasForeignKey<AsignacionTecnicoMedico>(d => d.TecnicoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Asignacio__Tecni__52593CB8");
        });

        modelBuilder.Entity<Dueno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Dueno__3214EC075489E667");

            entity.Property(e => e.Direccion).HasMaxLength(300);
            entity.Property(e => e.Latitud).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitud).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Especie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Especie__3214EC0741330A8E");

            entity.HasIndex(e => e.Nombre, "UQ__Especie__75E3EFCFE8202C87").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Paciente__3214EC072FB4B2DC");

            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FotoUrl).HasMaxLength(500);
            entity.Property(e => e.Nombre).HasMaxLength(80);

            entity.HasOne(d => d.Dueno).WithMany(p => p.Paciente)
                .HasForeignKey(d => d.DuenoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Paciente__DuenoI__5BE2A6F2");

            entity.HasOne(d => d.Especie).WithMany(p => p.Paciente)
                .HasForeignKey(d => d.EspecieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Paciente__Especi__59FA5E80");

            entity.HasOne(d => d.Medico).WithMany(p => p.Paciente)
                .HasForeignKey(d => d.MedicoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Paciente__Medico__5CD6CB2B");

            entity.HasOne(d => d.Raza).WithMany(p => p.Paciente)
                .HasForeignKey(d => d.RazaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Paciente__RazaId__5AEE82B9");
        });

        modelBuilder.Entity<Raza>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Raza__3214EC07DDFAB52F");

            entity.Property(e => e.Nombre).HasMaxLength(80);

            entity.HasOne(d => d.Especie).WithMany(p => p.Raza)
                .HasForeignKey(d => d.EspecieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Raza__EspecieId__403A8C7D");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC074FC392F0");

            entity.HasIndex(e => e.Token, "UQ__RefreshT__1EB4F817E75648E5").IsUnique();

            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Token).HasMaxLength(200);

            entity.HasOne(d => d.Usuario).WithMany(p => p.RefreshToken)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefreshTo__Usuar__4D94879B");
        });

        modelBuilder.Entity<RegistroVisita>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Registro__3214EC07A94777D8");

            entity.HasIndex(e => e.VisitaId, "UQ__Registro__D8D4BC43BB946772").IsUnique();

            entity.Property(e => e.Altura).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FotoUrl).HasMaxLength(500);
            entity.Property(e => e.Latitud).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitud).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Observaciones).HasMaxLength(1000);
            entity.Property(e => e.Peso).HasColumnType("decimal(7, 3)");
            entity.Property(e => e.Temperatura).HasColumnType("decimal(4, 1)");

            entity.HasOne(d => d.Visita).WithOne(p => p.RegistroVisita)
                .HasForeignKey<RegistroVisita>(d => d.VisitaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RegistroV__Visit__70DDC3D8");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rol__3214EC07E1F18E49");

            entity.HasIndex(e => e.Nombre, "UQ__Rol__75E3EFCFB648D13C").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Servicio__3214EC07FE91D108");

            entity.HasIndex(e => e.Nombre, "UQ__Servicio__75E3EFCF5AA94F34").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuario__3214EC07CF8D8B6C");

            entity.HasIndex(e => e.Email, "UQ__Usuario__A9D10534C309D0C0").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.PasswordHash).HasMaxLength(100);

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuario__RolId__46E78A0C");

            entity.HasOne(d => d.Zona).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.ZonaId)
                .HasConstraintName("FK__Usuario__ZonaId__47DBAE45");
        });

        modelBuilder.Entity<Visita>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Visita__3214EC07EC617BB8");

            entity.Property(e => e.Estado)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasDefaultValue("PENDIENTE");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MotivoCancelacion).HasMaxLength(300);

            entity.HasOne(d => d.CreadaPor).WithMany(p => p.VisitaCreadaPor)
                .HasForeignKey(d => d.CreadaPorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Visita__CreadaPo__6C190EBB");

            entity.HasOne(d => d.Paciente).WithMany(p => p.Visita)
                .HasForeignKey(d => d.PacienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Visita__Paciente__66603565");

            entity.HasOne(d => d.Servicio).WithMany(p => p.Visita)
                .HasForeignKey(d => d.ServicioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Visita__Servicio__6754599E");

            entity.HasOne(d => d.Tecnico).WithMany(p => p.VisitaTecnico)
                .HasForeignKey(d => d.TecnicoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Visita__TecnicoI__68487DD7");

            entity.HasOne(d => d.VisitaOrigen).WithMany(p => p.InverseVisitaOrigen)
                .HasForeignKey(d => d.VisitaOrigenId)
                .HasConstraintName("FK__Visita__VisitaOr__6B24EA82");
        });

        modelBuilder.Entity<Zona>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Zona__3214EC0737E0B0C1");

            entity.HasIndex(e => e.Nombre, "UQ__Zona__75E3EFCF272806CF").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(80);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
