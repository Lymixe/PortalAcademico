using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Identity;
using PortalAcademico.Models;
using System.Reflection.Emit;

namespace PortalAcademico.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Curso>(entity =>
            {
                entity.HasIndex(c => c.Codigo).IsUnique();
                entity.ToTable(tb => tb.HasCheckConstraint("CK_Curso_Creditos", "Creditos > 0"));
                entity.ToTable(tb => tb.HasCheckConstraint("CK_Curso_Horarios", "HorarioInicio < HorarioFin"));
            });

            builder.Entity<Matricula>(entity =>
            {
                entity.HasIndex(m => new { m.UsuarioId, m.CursoId }).IsUnique();
            });

            string coordinadorRoleId = "d1b5952a-2162-46c7-b29e-1a2a68922c14";
            string coordinadorUserId = "42efa459-7a54-449e-ae8a-335f63935a31";

            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = coordinadorRoleId,
                Name = "Coordinador",
                NormalizedName = "COORDINADOR"
            });

            var hasher = new PasswordHasher<IdentityUser>();
            builder.Entity<IdentityUser>().HasData(new IdentityUser
            {
                Id = coordinadorUserId,
                UserName = "coordinador@test.com",
                NormalizedUserName = "COORDINADOR@TEST.COM",
                Email = "coordinador@test.com",
                NormalizedEmail = "COORDINADOR@TEST.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Coordinador123!"),
                SecurityStamp = string.Empty
            });

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = coordinadorRoleId,
                UserId = coordinadorUserId
            });

            builder.Entity<Curso>().HasData(
                new Curso { Id = 1, Codigo = "CS101", Nombre = "Introducción a la Programación", Creditos = 4, CupoMaximo = 30, HorarioInicio = new TimeOnly(8, 0), HorarioFin = new TimeOnly(10, 0), Activo = true },
                new Curso { Id = 2, Codigo = "DB201", Nombre = "Bases de Datos Relacionales", Creditos = 5, CupoMaximo = 25, HorarioInicio = new TimeOnly(10, 0), HorarioFin = new TimeOnly(12, 0), Activo = true },
                new Curso { Id = 3, Codigo = "WEB301", Nombre = "Desarrollo de Aplicaciones Web", Creditos = 5, CupoMaximo = 20, HorarioInicio = new TimeOnly(14, 0), HorarioFin = new TimeOnly(16, 0), Activo = true }
            );
        }
    }
}