using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeInquiryAgentIdOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inquiries_Agents_AgentId",
                table: "Inquiries");

            migrationBuilder.DropForeignKey(
                name: "FK_Inquiries_AspNetUsers_BuyerId",
                table: "Inquiries");

            migrationBuilder.AlterColumn<Guid>(
                name: "AgentId",
                table: "Inquiries",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Inquiries_Agents_AgentId",
                table: "Inquiries",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Inquiries_AspNetUsers_BuyerId",
                table: "Inquiries",
                column: "BuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inquiries_Agents_AgentId",
                table: "Inquiries");

            migrationBuilder.DropForeignKey(
                name: "FK_Inquiries_AspNetUsers_BuyerId",
                table: "Inquiries");

            migrationBuilder.AlterColumn<Guid>(
                name: "AgentId",
                table: "Inquiries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Inquiries_Agents_AgentId",
                table: "Inquiries",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inquiries_AspNetUsers_BuyerId",
                table: "Inquiries",
                column: "BuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
