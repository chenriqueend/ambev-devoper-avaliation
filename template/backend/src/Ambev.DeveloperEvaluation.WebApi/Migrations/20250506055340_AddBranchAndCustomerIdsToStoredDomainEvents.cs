using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchAndCustomerIdsToStoredDomainEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "StoredDomainEvents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "StoredDomainEvents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoredDomainEvents_BranchId",
                table: "StoredDomainEvents",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_StoredDomainEvents_CustomerId",
                table: "StoredDomainEvents",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoredDomainEvents_Branches_BranchId",
                table: "StoredDomainEvents",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StoredDomainEvents_Customers_CustomerId",
                table: "StoredDomainEvents",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoredDomainEvents_Branches_BranchId",
                table: "StoredDomainEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_StoredDomainEvents_Customers_CustomerId",
                table: "StoredDomainEvents");

            migrationBuilder.DropIndex(
                name: "IX_StoredDomainEvents_BranchId",
                table: "StoredDomainEvents");

            migrationBuilder.DropIndex(
                name: "IX_StoredDomainEvents_CustomerId",
                table: "StoredDomainEvents");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "StoredDomainEvents");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "StoredDomainEvents");
        }
    }
}
