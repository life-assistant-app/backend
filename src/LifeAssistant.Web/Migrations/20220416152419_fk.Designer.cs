﻿// <auto-generated />
using System;
using LifeAssistant.Web.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LifeAssistant.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220416152419_fk")]
    partial class fk
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LifeAssistant.Web.Database.Entities.ApplicationUserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Validated")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("LifeAssistant.Web.Database.Entities.AppointmentEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ApplicationUserEntityId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("LifeAssistantId")
                        .HasColumnType("uuid");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserEntityId");

                    b.HasIndex("LifeAssistantId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("LifeAssistant.Web.Database.Entities.AppointmentEntity", b =>
                {
                    b.HasOne("LifeAssistant.Web.Database.Entities.ApplicationUserEntity", null)
                        .WithMany("Appointments")
                        .HasForeignKey("ApplicationUserEntityId");

                    b.HasOne("LifeAssistant.Web.Database.Entities.ApplicationUserEntity", null)
                        .WithMany()
                        .HasForeignKey("LifeAssistantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LifeAssistant.Web.Database.Entities.ApplicationUserEntity", b =>
                {
                    b.Navigation("Appointments");
                });
#pragma warning restore 612, 618
        }
    }
}
