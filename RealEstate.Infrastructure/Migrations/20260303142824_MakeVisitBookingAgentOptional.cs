using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeVisitBookingAgentOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitBookings_Agents_AgentId",
                table: "VisitBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitBookings_AspNetUsers_BuyerId",
                table: "VisitBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitBookings_Properties_PropertyId",
                table: "VisitBookings");

            migrationBuilder.AlterColumn<Guid>(
                name: "AgentId",
                table: "VisitBookings",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitBookings_Agents_AgentId",
                table: "VisitBookings",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitBookings_AspNetUsers_BuyerId",
                table: "VisitBookings",
                column: "BuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitBookings_Properties_PropertyId",
                table: "VisitBookings",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitBookings_Agents_AgentId",
                table: "VisitBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitBookings_AspNetUsers_BuyerId",
                table: "VisitBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitBookings_Properties_PropertyId",
                table: "VisitBookings");

            migrationBuilder.AlterColumn<Guid>(
                name: "AgentId",
                table: "VisitBookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitBookings_Agents_AgentId",
                table: "VisitBookings",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitBookings_AspNetUsers_BuyerId",
                table: "VisitBookings",
                column: "BuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitBookings_Properties_PropertyId",
                table: "VisitBookings",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
