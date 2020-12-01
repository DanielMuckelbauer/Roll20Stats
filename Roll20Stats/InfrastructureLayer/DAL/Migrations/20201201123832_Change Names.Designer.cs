﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Roll20Stats.InfrastructureLayer.DAL.Context;

namespace Roll20Stats.InfrastructureLayer.DAL.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20201201123832_Change Names")]
    partial class ChangeNames
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Roll20Stats.InfrastructureLayer.DAL.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Roll20Stats.InfrastructureLayer.DAL.Models.PlayerStatistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("CharacterId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CharacterName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DamageDealt")
                        .HasColumnType("int");

                    b.Property<int>("DamageTaken")
                        .HasColumnType("int");

                    b.Property<int?>("GameId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("PlayerStatistics");
                });

            modelBuilder.Entity("Roll20Stats.InfrastructureLayer.DAL.Models.PlayerStatistic", b =>
                {
                    b.HasOne("Roll20Stats.InfrastructureLayer.DAL.Models.Game", null)
                        .WithMany("PlayerStats")
                        .HasForeignKey("GameId");
                });

            modelBuilder.Entity("Roll20Stats.InfrastructureLayer.DAL.Models.Game", b =>
                {
                    b.Navigation("PlayerStats");
                });
#pragma warning restore 612, 618
        }
    }
}