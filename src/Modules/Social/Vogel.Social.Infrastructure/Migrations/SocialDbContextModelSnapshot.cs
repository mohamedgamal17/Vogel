﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vogel.Social.Infrastructure.EntityFramework;

#nullable disable

namespace Vogel.Social.Infrastructure.Migrations
{
    [DbContext(typeof(SocialDbContext))]
    partial class SocialDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Social")
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Vogel.Social.Domain.Pictures.Picture", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatorId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime?>("DeletionTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeletorId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("File")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifierId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("DeletorId");

                    b.HasIndex("ModifierId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Pictures", "Social");
                });

            modelBuilder.Entity("Vogel.Social.Domain.Users.User", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("AvatarId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("date");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatorId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime?>("DeletionTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeletorId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifierId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("AvatarId")
                        .IsUnique()
                        .HasFilter("[AvatarId] IS NOT NULL");

                    b.HasIndex("CreatorId");

                    b.HasIndex("DeletorId");

                    b.HasIndex("ModifierId");

                    b.ToTable("Users", "Social");
                });

            modelBuilder.Entity("Vogel.Social.Domain.Users.User", b =>
                {
                    b.HasOne("Vogel.Social.Domain.Pictures.Picture", null)
                        .WithOne()
                        .HasForeignKey("Vogel.Social.Domain.Users.User", "AvatarId");
                });
#pragma warning restore 612, 618
        }
    }
}