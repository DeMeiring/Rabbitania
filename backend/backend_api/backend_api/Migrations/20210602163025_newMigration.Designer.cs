﻿// <auto-generated />
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using backend_api.Models;

namespace backend_api.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20210602163025_newMigration")]
    partial class newMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "6.0.0-preview.4.21253.1")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("backend_api.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("employeeLevel")
                        .HasColumnType("integer");

                    b.Property<string>("firstname")
                        .HasColumnType("text");

                    b.Property<bool>("isAdmin")
                        .HasColumnType("boolean");

                    b.Property<bool>("isOnline")
                        .HasColumnType("boolean");

                    b.Property<string>("lastname")
                        .HasColumnType("text");

                    b.Property<int>("officeLocation")
                        .HasColumnType("integer");

                    b.Property<string>("phoneNumber")
                        .HasColumnType("text");

                    b.Property<List<int>>("pinnedUserIDs")
                        .HasColumnType("integer[]");

                    b.Property<string>("userDescription")
                        .HasColumnType("text");

                    b.Property<int>("userEmailsID")
                        .HasColumnType("integer");

                    b.Property<string>("userImage")
                        .HasColumnType("text");

                    b.Property<int>("userRoles")
                        .HasColumnType("integer");

                    b.HasKey("UserID");

                    b.HasIndex("userEmailsID");

                    b.ToTable("users");
                });

            modelBuilder.Entity("backend_api.Models.UserEmails", b =>
                {
                    b.Property<int>("userEmailsID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("userEmail")
                        .HasColumnType("text");

                    b.HasKey("userEmailsID");

                    b.ToTable("UserEmails");
                });

            modelBuilder.Entity("backend_api.Models.User", b =>
                {
                    b.HasOne("backend_api.Models.UserEmails", "UserEmails")
                        .WithMany()
                        .HasForeignKey("userEmailsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserEmails");
                });
#pragma warning restore 612, 618
        }
    }
}
