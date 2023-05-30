﻿// <auto-generated />
using System;
using Librarian.Sephirah.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Librarian.Sephirah.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("AppUser", b =>
                {
                    b.Property<long>("AppsId")
                        .HasColumnType("bigint");

                    b.Property<long>("UsersId")
                        .HasColumnType("bigint");

                    b.HasKey("AppsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("AppUser");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.App", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("ShortDescription")
                        .HasMaxLength(1024)
                        .HasColumnType("varchar(1024)");

                    b.Property<int>("Source")
                        .HasColumnType("int");

                    b.Property<string>("SourceAppId")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("SourceUrl")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("Name");

                    b.HasIndex("Source");

                    b.HasIndex("SourceAppId");

                    b.HasIndex("Type");

                    b.HasIndex("UpdatedAt");

                    b.ToTable("Apps");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.AppDetails", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<long>("AppId")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("Developer")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("Publisher")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<DateTime?>("ReleaseDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Version")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.HasKey("Id");

                    b.HasIndex("AppId")
                        .IsUnique();

                    b.ToTable("AppDetails");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.AppPackage", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<long>("AppId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasMaxLength(1024)
                        .HasColumnType("varchar(1024)");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<int>("Source")
                        .HasColumnType("int");

                    b.Property<long>("SourceAppId")
                        .HasColumnType("bigint");

                    b.Property<TimeSpan>("TotalRunTime")
                        .HasColumnType("time(6)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("AppId");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("Source");

                    b.HasIndex("SourceAppId");

                    b.HasIndex("UpdatedAt");

                    b.ToTable("AppPackages");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.AppPackageBinary", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<long>("AppPackageId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("PublicUrl")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<byte[]>("Sha256")
                        .HasMaxLength(32)
                        .HasColumnType("binary(32)")
                        .IsFixedLength();

                    b.Property<long>("SizeBytes")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("AppPackageId")
                        .IsUnique();

                    b.HasIndex("CreatedAt");

                    b.HasIndex("UpdatedAt");

                    b.ToTable("AppPackagesBinaries");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.AppPackageRunTime", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("AppPackageId")
                        .HasColumnType("bigint");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time(6)");

                    b.Property<DateTime>("StartDateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("AppPackageId");

                    b.ToTable("AppPackageRunTimes");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.FileMetadata", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<byte[]>("Sha256")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("binary(32)")
                        .IsFixedLength();

                    b.Property<long>("SizeBytes")
                        .HasColumnType("bigint");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("UpdatedAt");

                    b.ToTable("FileMetadatas");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.GameSaveFile", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<long>("AppPackageId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("FileMetadataId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsPinned")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AppPackageId");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("FileMetadataId")
                        .IsUnique();

                    b.HasIndex("UpdatedAt");

                    b.HasIndex("UserId");

                    b.ToTable("GameSaveFiles");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.GameSaveFileRotation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("Count")
                        .HasColumnType("bigint");

                    b.Property<long>("EntityInternalId")
                        .HasColumnType("bigint");

                    b.Property<int>("VaildScope")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EntityInternalId");

                    b.ToTable("GameSaveFileRotations");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<long?>("GameSaveFileCapacityBytes")
                        .HasColumnType("bigint");

                    b.Property<long>("GameSaveFileUsedCapacityBytes")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("UpdatedAt");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AppUser", b =>
                {
                    b.HasOne("Librarian.Sephirah.Models.App", null)
                        .WithMany()
                        .HasForeignKey("AppsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Librarian.Sephirah.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.AppDetails", b =>
                {
                    b.HasOne("Librarian.Sephirah.Models.App", "App")
                        .WithOne("AppDetails")
                        .HasForeignKey("Librarian.Sephirah.Models.AppDetails", "AppId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("App");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.AppPackage", b =>
                {
                    b.HasOne("Librarian.Sephirah.Models.App", "App")
                        .WithMany("AppPackages")
                        .HasForeignKey("AppId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("App");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.AppPackageBinary", b =>
                {
                    b.HasOne("Librarian.Sephirah.Models.AppPackage", "AppPackage")
                        .WithOne("AppPackageBinary")
                        .HasForeignKey("Librarian.Sephirah.Models.AppPackageBinary", "AppPackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppPackage");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.AppPackageRunTime", b =>
                {
                    b.HasOne("Librarian.Sephirah.Models.AppPackage", "AppPackage")
                        .WithMany()
                        .HasForeignKey("AppPackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppPackage");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.GameSaveFile", b =>
                {
                    b.HasOne("Librarian.Sephirah.Models.AppPackage", "AppPackage")
                        .WithMany()
                        .HasForeignKey("AppPackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Librarian.Sephirah.Models.FileMetadata", "FileMetadata")
                        .WithOne("GameSaveFile")
                        .HasForeignKey("Librarian.Sephirah.Models.GameSaveFile", "FileMetadataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Librarian.Sephirah.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppPackage");

                    b.Navigation("FileMetadata");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.App", b =>
                {
                    b.Navigation("AppDetails");

                    b.Navigation("AppPackages");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.AppPackage", b =>
                {
                    b.Navigation("AppPackageBinary");
                });

            modelBuilder.Entity("Librarian.Sephirah.Models.FileMetadata", b =>
                {
                    b.Navigation("GameSaveFile");
                });
#pragma warning restore 612, 618
        }
    }
}
