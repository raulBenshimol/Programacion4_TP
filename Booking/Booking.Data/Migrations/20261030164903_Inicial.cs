using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Booking.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SolicitudId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AsientoId = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Monto = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MotivoRechazo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Version = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_Estado_FechaVencimiento",
                table: "Reservas",
                columns: new[] { "Estado", "FechaVencimiento" });

            migrationBuilder.CreateIndex(
                name: "UX_Reservas_EventoId_AsientoId_Activo",
                table: "Reservas",
                columns: new[] { "EventoId", "AsientoId" },
                unique: true,
                filter: "\"Estado\" IN ('Retenida', 'Confirmada')");

            migrationBuilder.CreateIndex(
                name: "UX_Reservas_SolicitudId",
                table: "Reservas",
                column: "SolicitudId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservas");
        }
    }
}
