﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace razor_web.Migrations
{
    /// <inheritdoc />
    public partial class seedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            for(int i = 1; i < 150 ; i++){
                migrationBuilder.InsertData(
                    "Users",
                    columns: new[] {
                        "Id",
                        "UserName",
                        "Email",
                        "SecurityStamp",
                        "EmailConfirmed",
                        "PhoneNumberConfirmed",
                        "TwoFactorEnabled",
                        "LockoutEnabled",
                        "AccessFailedCount",
                        "HomeAddress",
                    },
                    values: new object[]{
                        Guid.NewGuid().ToString(),
                        "User-"+i.ToString("D3"),
                        $"Email{i.ToString("D3")}@example.com",
                        Guid.NewGuid().ToString(),
                        true,
                        false,
                        false,
                        false,  
                        0,
                        "...@#%..."
                    }
                );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
