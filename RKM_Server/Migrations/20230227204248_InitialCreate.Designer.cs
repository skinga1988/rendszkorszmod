﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RKM_Server.Data;

#nullable disable

namespace RKM_Server.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230227204248_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("RKM_Server.Models.Orderer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrdererName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Orderers");
                });

            modelBuilder.Entity("RKM_Server.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("OrdererId")
                        .HasColumnType("int");

                    b.Property<string>("Place")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StockAccountId")
                        .HasColumnType("int");

                    b.Property<int?>("StockAccountUserId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id", "UserId");

                    b.HasIndex("OrdererId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.HasIndex("StockAccountId", "StockAccountUserId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("RKM_Server.Models.ProjectAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("ProjectId1")
                        .HasColumnType("int");

                    b.Property<int>("ProjectUserId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("ProjectId1", "ProjectUserId");

                    b.ToTable("ProjectAccounts");
                });

            modelBuilder.Entity("RKM_Server.Models.Stock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AvailablePieces")
                        .HasColumnType("int");

                    b.Property<int>("BoxId")
                        .HasColumnType("int");

                    b.Property<int>("ColumnId")
                        .HasColumnType("int");

                    b.Property<int>("MaxPieces")
                        .HasColumnType("int");

                    b.Property<int>("ReservedPieces")
                        .HasColumnType("int");

                    b.Property<int>("RowId")
                        .HasColumnType("int");

                    b.Property<int>("StockItemId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("StockItemId");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("RKM_Server.Models.StockAccount", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("AccountTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Pieces")
                        .HasColumnType("int");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("ProjectId1")
                        .HasColumnType("int");

                    b.Property<int>("ProjectUserId")
                        .HasColumnType("int");

                    b.Property<int?>("StockId")
                        .HasColumnType("int");

                    b.Property<int>("StockItemId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id", "UserId");

                    b.HasIndex("StockId");

                    b.HasIndex("StockItemId");

                    b.HasIndex("UserId");

                    b.HasIndex("ProjectId1", "ProjectUserId");

                    b.ToTable("StockAccounts");
                });

            modelBuilder.Entity("RKM_Server.Models.StockItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ItemPrice")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("StockItems");
                });

            modelBuilder.Entity("RKM_Server.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"), 1L, 1);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RKM_Server.Models.Orderer", b =>
                {
                    b.HasOne("RKM_Server.Models.User", "User")
                        .WithMany("Orderers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("RKM_Server.Models.Project", b =>
                {
                    b.HasOne("RKM_Server.Models.Orderer", "Orderer")
                        .WithOne("Project")
                        .HasForeignKey("RKM_Server.Models.Project", "OrdererId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RKM_Server.Models.User", "User")
                        .WithMany("Projects")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("RKM_Server.Models.StockAccount", null)
                        .WithMany("Projects")
                        .HasForeignKey("StockAccountId", "StockAccountUserId");

                    b.Navigation("Orderer");

                    b.Navigation("User");
                });

            modelBuilder.Entity("RKM_Server.Models.ProjectAccount", b =>
                {
                    b.HasOne("RKM_Server.Models.User", null)
                        .WithMany("ProjecAccounts")
                        .HasForeignKey("UserId");

                    b.HasOne("RKM_Server.Models.Project", "Project")
                        .WithMany("ProjectAccounts")
                        .HasForeignKey("ProjectId1", "ProjectUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("RKM_Server.Models.Stock", b =>
                {
                    b.HasOne("RKM_Server.Models.StockItem", "StockItem")
                        .WithMany("Stocks")
                        .HasForeignKey("StockItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StockItem");
                });

            modelBuilder.Entity("RKM_Server.Models.StockAccount", b =>
                {
                    b.HasOne("RKM_Server.Models.Stock", null)
                        .WithMany("StockAccounts")
                        .HasForeignKey("StockId");

                    b.HasOne("RKM_Server.Models.StockItem", "StockItem")
                        .WithMany("StocksAccounts")
                        .HasForeignKey("StockItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RKM_Server.Models.User", "User")
                        .WithMany("StockAccounts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("RKM_Server.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId1", "ProjectUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("StockItem");

                    b.Navigation("User");
                });

            modelBuilder.Entity("RKM_Server.Models.Orderer", b =>
                {
                    b.Navigation("Project")
                        .IsRequired();
                });

            modelBuilder.Entity("RKM_Server.Models.Project", b =>
                {
                    b.Navigation("ProjectAccounts");
                });

            modelBuilder.Entity("RKM_Server.Models.Stock", b =>
                {
                    b.Navigation("StockAccounts");
                });

            modelBuilder.Entity("RKM_Server.Models.StockAccount", b =>
                {
                    b.Navigation("Projects");
                });

            modelBuilder.Entity("RKM_Server.Models.StockItem", b =>
                {
                    b.Navigation("Stocks");

                    b.Navigation("StocksAccounts");
                });

            modelBuilder.Entity("RKM_Server.Models.User", b =>
                {
                    b.Navigation("Orderers");

                    b.Navigation("ProjecAccounts");

                    b.Navigation("Projects");

                    b.Navigation("StockAccounts");
                });
#pragma warning restore 612, 618
        }
    }
}