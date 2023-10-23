﻿// <auto-generated />
using System;
using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Librarian.Sephirah.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231023020516_UpdateName")]
    partial class UpdateName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
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

            modelBuilder.Entity("Librarian.Common.Models.App", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("HeroImageUrl")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("IconImageUrl")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<long?>("ParentAppId")
                        .HasColumnType("bigint");

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

                    b.HasIndex("ParentAppId");

                    b.HasIndex("Source");

                    b.HasIndex("SourceAppId");

                    b.HasIndex("Type");

                    b.HasIndex("UpdatedAt");

                    b.ToTable("Apps");
                });

            modelBuilder.Entity("Librarian.Common.Models.AppCategory", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("UpdatedAt");

                    b.HasIndex("UserId");

                    b.ToTable("AppCategories");
                });

            modelBuilder.Entity("Librarian.Common.Models.AppDetails", b =>
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

            modelBuilder.Entity("Librarian.Common.Models.AppPackage", b =>
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

            modelBuilder.Entity("Librarian.Common.Models.AppPackageBinary", b =>
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

            modelBuilder.Entity("Librarian.Common.Models.FileMetadata", b =>
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

            modelBuilder.Entity("Librarian.Common.Models.GameSaveFile", b =>
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

            modelBuilder.Entity("Librarian.Common.Models.GameSaveFileRotation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("Count")
                        .HasColumnType("bigint");

                    b.Property<long?>("EntityInternalId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<int>("VaildScope")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EntityInternalId");

                    b.HasIndex("UserId");

                    b.ToTable("GameSaveFileRotations");
                });

            modelBuilder.Entity("Librarian.Common.Models.User", b =>
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

            modelBuilder.Entity("Librarian.Common.Models.UserAppAppCategory", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("AppId")
                        .HasColumnType("bigint");

                    b.Property<long>("AppCategoryId")
                        .HasColumnType("bigint");

                    b.HasKey("UserId", "AppId", "AppCategoryId");

                    b.HasIndex("AppCategoryId");

                    b.HasIndex("AppId");

                    b.ToTable("UserAppAppCategories");
                });

            modelBuilder.Entity("Librarian.Common.Models.UserAppPackage", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("AppPackageId")
                        .HasColumnType("bigint");

                    b.Property<TimeSpan>("TotalRunTime")
                        .HasColumnType("time(6)");

                    b.HasKey("UserId", "AppPackageId");

                    b.HasIndex("AppPackageId");

                    b.ToTable("UserAppPackages");
                });

            modelBuilder.Entity("Librarian.Common.Models.UserAppPackageRunTime", b =>
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

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AppPackageId");

                    b.HasIndex("UserId");

                    b.ToTable("UserAppPackageRunTimes");
                });

            modelBuilder.Entity("AppUser", b =>
                {
                    b.HasOne("Librarian.Common.Models.App", null)
                        .WithMany()
                        .HasForeignKey("AppsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Librarian.Common.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Librarian.Common.Models.App", b =>
                {
                    b.HasOne("Librarian.Common.Models.App", "ParentApp")
                        .WithMany("ChildApps")
                        .HasForeignKey("ParentAppId");

                    b.Navigation("ParentApp");
                });

            modelBuilder.Entity("Librarian.Common.Models.AppCategory", b =>
                {
                    b.HasOne("Librarian.Common.Models.User", "User")
                        .WithMany("AppCategories")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Librarian.Common.Models.AppDetails", b =>
                {
                    b.HasOne("Librarian.Common.Models.App", "App")
                        .WithOne("AppDetails")
                        .HasForeignKey("Librarian.Common.Models.AppDetails", "AppId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("App");
                });

            modelBuilder.Entity("Librarian.Common.Models.AppPackage", b =>
                {
                    b.HasOne("Librarian.Common.Models.App", "App")
                        .WithMany("AppPackages")
                        .HasForeignKey("AppId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("App");
                });

            modelBuilder.Entity("Librarian.Common.Models.AppPackageBinary", b =>
                {
                    b.HasOne("Librarian.Common.Models.AppPackage", "AppPackage")
                        .WithOne("AppPackageBinary")
                        .HasForeignKey("Librarian.Common.Models.AppPackageBinary", "AppPackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppPackage");
                });

            modelBuilder.Entity("Librarian.Common.Models.GameSaveFile", b =>
                {
                    b.HasOne("Librarian.Common.Models.AppPackage", "AppPackage")
                        .WithMany()
                        .HasForeignKey("AppPackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Librarian.Common.Models.FileMetadata", "FileMetadata")
                        .WithOne("GameSaveFile")
                        .HasForeignKey("Librarian.Common.Models.GameSaveFile", "FileMetadataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Librarian.Common.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppPackage");

                    b.Navigation("FileMetadata");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Librarian.Common.Models.GameSaveFileRotation", b =>
                {
                    b.HasOne("Librarian.Common.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Librarian.Common.Models.UserAppAppCategory", b =>
                {
                    b.HasOne("Librarian.Common.Models.AppCategory", "AppCategory")
                        .WithMany()
                        .HasForeignKey("AppCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Librarian.Common.Models.App", "App")
                        .WithMany()
                        .HasForeignKey("AppId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Librarian.Common.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("App");

                    b.Navigation("AppCategory");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Librarian.Common.Models.UserAppPackage", b =>
                {
                    b.HasOne("Librarian.Common.Models.AppPackage", "AppPackage")
                        .WithMany()
                        .HasForeignKey("AppPackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Librarian.Common.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppPackage");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Librarian.Common.Models.UserAppPackageRunTime", b =>
                {
                    b.HasOne("Librarian.Common.Models.AppPackage", "AppPackage")
                        .WithMany()
                        .HasForeignKey("AppPackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Librarian.Common.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppPackage");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Librarian.Common.Models.App", b =>
                {
                    b.Navigation("AppDetails");

                    b.Navigation("AppPackages");

                    b.Navigation("ChildApps");
                });

            modelBuilder.Entity("Librarian.Common.Models.AppPackage", b =>
                {
                    b.Navigation("AppPackageBinary");
                });

            modelBuilder.Entity("Librarian.Common.Models.FileMetadata", b =>
                {
                    b.Navigation("GameSaveFile");
                });

            modelBuilder.Entity("Librarian.Common.Models.User", b =>
                {
                    b.Navigation("AppCategories");
                });
#pragma warning restore 612, 618
        }
    }
}
