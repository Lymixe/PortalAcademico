using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PortalAcademico.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialDomainSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Creditos = table.Column<int>(type: "INTEGER", nullable: false),
                    CupoMaximo = table.Column<int>(type: "INTEGER", nullable: false),
                    HorarioInicio = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    HorarioFin = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                    table.CheckConstraint("CK_Curso_Creditos", "Creditos > 0");
                    table.CheckConstraint("CK_Curso_Horarios", "HorarioInicio < HorarioFin");
                });

            migrationBuilder.CreateTable(
                name: "Matriculas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CursoId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matriculas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matriculas_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matriculas_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d1b5952a-2162-46c7-b29e-1a2a68922c14", null, "Coordinador", "COORDINADOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "42efa459-7a54-449e-ae8a-335f63935a31", 0, "b8a8047b-2321-4d30-9b34-8f430538f296", "coordinador@test.com", true, false, null, "COORDINADOR@TEST.COM", "COORDINADOR@TEST.COM", "AQAAAAIAAYagAAAAEJxG0bQD0zI2VnhkO6rkTC44A5Gz/5sMG4s/G/z53e246a48x5pZkGu3XyFw0s1v9Q==", null, false, "YHMJDCG2W44O7GKIQ3AJNUCBQP2V24J2", false, "coordinador@test.com" });

            migrationBuilder.InsertData(
                table: "Cursos",
                columns: new[] { "Id", "Activo", "Codigo", "Creditos", "CupoMaximo", "HorarioFin", "HorarioInicio", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "CS101", 4, 30, new TimeOnly(10, 0, 0), new TimeOnly(8, 0, 0), "Introducción a la Programación" },
                    { 2, true, "DB201", 5, 25, new TimeOnly(12, 0, 0), new TimeOnly(10, 0, 0), "Bases de Datos Relacionales" },
                    { 3, true, "WEB301", 5, 20, new TimeOnly(16, 0, 0), new TimeOnly(14, 0, 0), "Desarrollo de Aplicaciones Web" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "d1b5952a-2162-46c7-b29e-1a2a68922c14", "42efa459-7a54-449e-ae8a-335f63935a31" });

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_Codigo",
                table: "Cursos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_CursoId",
                table: "Matriculas",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_UsuarioId_CursoId",
                table: "Matriculas",
                columns: new[] { "UsuarioId", "CursoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matriculas");

            migrationBuilder.DropTable(
                name: "Cursos");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "d1b5952a-2162-46c7-b29e-1a2a68922c14", "42efa459-7a54-449e-ae8a-335f63935a31" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d1b5952a-2162-46c7-b29e-1a2a68922c14");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "42efa459-7a54-449e-ae8a-335f63935a31");
        }
    }
}
