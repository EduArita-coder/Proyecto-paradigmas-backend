using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GAMEHOSTING_APIREST.Migrations
{
    /// <inheritdoc />
    public partial class SyncPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0001-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0001-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0001-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0001-0000-0000-000000000004"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "Id", "Cpu", "CreatedAt", "Description", "ImageUrl", "Name", "Price", "Ram", "Slots" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-0001-0000-0000-000000000001"), "Shared vCPU", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Servidor vanilla para grupos pequeños. 2 GB RAM, 10 slots.", "/images/minecraft.png", "Minecraft - Plan Básico", 4.99m, "2 GB", 10 },
                    { new Guid("a1b2c3d4-0001-0000-0000-000000000002"), "4 vCPU", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Servidor con mods y plugins. 8 GB RAM, 50 slots.", "/images/minecraft.png", "Minecraft - Plan Premium", 14.99m, "8 GB", 50 },
                    { new Guid("a1b2c3d4-0001-0000-0000-000000000003"), "4 vCPU", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Servidor dedicado para Rust. 8 GB RAM, 100 slots.", "/images/rust.png", "Rust - Plan Estándar", 19.99m, "8 GB", 100 },
                    { new Guid("a1b2c3d4-0001-0000-0000-000000000004"), "2 vCPU", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Servidor 128 tick para partidas competitivas. 4 GB RAM.", "/images/cs2.png", "CS2 - Competitivo", 9.99m, "4 GB", 16 }
                });
        }
    }
}
