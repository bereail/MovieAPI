﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using movie_api.Data;

#nullable disable

namespace movie_api.Migrations
{
    [DbContext(typeof(movieDbContext))]
    [Migration("20231220124916_UpdateReturnDateNullable")]
    partial class UpdateReturnDateNullable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MOVIE_API.Models.Admin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("EmployeeNum")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("employee_num");

                    b.Property<int>("IdUser")
                        .HasColumnType("int");

                    b.Property<int?>("IdUserNavigationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdUserNavigationId");

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("MOVIE_API.Models.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("date");

                    b.Property<int>("IdUser")
                        .HasColumnType("int");

                    b.Property<int?>("IdUserNavigationId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ReturnDate")
                        .HasColumnType("date");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdUserNavigationId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("MOVIE_API.Models.BookingDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("date");

                    b.Property<string>("Comment")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("comment");

                    b.Property<int>("IdBooking")
                        .HasColumnType("int");

                    b.Property<int?>("IdBookingNavigationId")
                        .HasColumnType("int");

                    b.Property<int>("IdMovie")
                        .HasColumnType("int");

                    b.Property<int?>("IdMovieNavigationId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReturnDate")
                        .HasColumnType("date");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdBookingNavigationId");

                    b.HasIndex("IdMovieNavigationId");

                    b.ToTable("BookingDetails");
                });

            modelBuilder.Entity("MOVIE_API.Models.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("IdUser")
                        .HasColumnType("int");

                    b.Property<int?>("IdUserNavigationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdUserNavigationId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("MOVIE_API.Models.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Director")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("director");

                    b.Property<int>("IdAdmin")
                        .HasColumnType("int");

                    b.Property<int?>("IdAdminNavigationId")
                        .HasColumnType("int");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("title");

                    b.HasKey("Id");

                    b.HasIndex("IdAdminNavigationId");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("MOVIE_API.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("email");

                    b.Property<string>("Lastname")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("lastname");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("name");

                    b.Property<string>("Pass")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("pass");

                    b.Property<string>("Rol")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("rol");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MOVIE_API.Models.Admin", b =>
                {
                    b.HasOne("MOVIE_API.Models.User", "IdUserNavigation")
                        .WithMany("Admins")
                        .HasForeignKey("IdUserNavigationId");

                    b.Navigation("IdUserNavigation");
                });

            modelBuilder.Entity("MOVIE_API.Models.Booking", b =>
                {
                    b.HasOne("MOVIE_API.Models.User", "IdUserNavigation")
                        .WithMany("Bookings")
                        .HasForeignKey("IdUserNavigationId");

                    b.Navigation("IdUserNavigation");
                });

            modelBuilder.Entity("MOVIE_API.Models.BookingDetail", b =>
                {
                    b.HasOne("MOVIE_API.Models.Booking", "IdBookingNavigation")
                        .WithMany("BookingDetails")
                        .HasForeignKey("IdBookingNavigationId");

                    b.HasOne("MOVIE_API.Models.Movie", "IdMovieNavigation")
                        .WithMany("BookingDetails")
                        .HasForeignKey("IdMovieNavigationId");

                    b.Navigation("IdBookingNavigation");

                    b.Navigation("IdMovieNavigation");
                });

            modelBuilder.Entity("MOVIE_API.Models.Client", b =>
                {
                    b.HasOne("MOVIE_API.Models.User", "IdUserNavigation")
                        .WithMany("Clients")
                        .HasForeignKey("IdUserNavigationId");

                    b.Navigation("IdUserNavigation");
                });

            modelBuilder.Entity("MOVIE_API.Models.Movie", b =>
                {
                    b.HasOne("MOVIE_API.Models.Admin", "IdAdminNavigation")
                        .WithMany("Movies")
                        .HasForeignKey("IdAdminNavigationId");

                    b.Navigation("IdAdminNavigation");
                });

            modelBuilder.Entity("MOVIE_API.Models.Admin", b =>
                {
                    b.Navigation("Movies");
                });

            modelBuilder.Entity("MOVIE_API.Models.Booking", b =>
                {
                    b.Navigation("BookingDetails");
                });

            modelBuilder.Entity("MOVIE_API.Models.Movie", b =>
                {
                    b.Navigation("BookingDetails");
                });

            modelBuilder.Entity("MOVIE_API.Models.User", b =>
                {
                    b.Navigation("Admins");

                    b.Navigation("Bookings");

                    b.Navigation("Clients");
                });
#pragma warning restore 612, 618
        }
    }
}
