﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using backend.Data;

#nullable disable

namespace backend.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("backend.Models.Movie", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("boolean");

                    b.Property<string>("Overview")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("PosterPath")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<double>("Rating")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ReviewDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Id");

                    b.ToTable("Movies");
                });
#pragma warning restore 612, 618
        }
    }
}
