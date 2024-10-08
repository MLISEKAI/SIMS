﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SIMS.Data;

#nullable disable

namespace SIMS.Migrations
{
    [DbContext(typeof(SIMSContext))]
    partial class SIMSContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.32")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("SIMS.AdminSystem", b =>
                {
                    b.Property<int>("Admin_ID")
                        .HasColumnType("int");

                    b.HasKey("Admin_ID");

                    b.ToTable("AdminSystem");
                });

            modelBuilder.Entity("SIMS.Courses", b =>
                {
                    b.Property<int>("Course_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Course_ID"), 1L, 1);

                    b.Property<string>("Course_Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Course_code")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Course_ID");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("SIMS.Scores", b =>
                {
                    b.Property<int>("Score_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Score_ID"), 1L, 1);

                    b.Property<int>("Course_ID")
                        .HasColumnType("int");

                    b.Property<int?>("CoursesCourse_ID")
                        .HasColumnType("int");

                    b.Property<string>("Grade")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Score")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Student_ID")
                        .HasColumnType("int");

                    b.Property<int?>("StudentsStudent_ID")
                        .HasColumnType("int");

                    b.Property<int>("Teacher_ID")
                        .HasColumnType("int");

                    b.Property<int?>("TeachersTeacher_ID")
                        .HasColumnType("int");

                    b.HasKey("Score_ID");

                    b.HasIndex("Course_ID");

                    b.HasIndex("CoursesCourse_ID");

                    b.HasIndex("Student_ID");

                    b.HasIndex("StudentsStudent_ID");

                    b.HasIndex("Teacher_ID");

                    b.HasIndex("TeachersTeacher_ID");

                    b.ToTable("Scores");
                });

            modelBuilder.Entity("SIMS.Students", b =>
                {
                    b.Property<int>("Student_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Student_ID"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<string>("Student_Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Student_ID");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("SIMS.Students_Courses", b =>
                {
                    b.Property<int>("Student_ID")
                        .HasColumnType("int");

                    b.Property<int>("Course_ID")
                        .HasColumnType("int");

                    b.HasKey("Student_ID", "Course_ID");

                    b.HasIndex("Course_ID");

                    b.ToTable("Students_Courses");
                });

            modelBuilder.Entity("SIMS.Teachers", b =>
                {
                    b.Property<int>("Teacher_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Teacher_ID"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<string>("Teacher_Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Teacher_ID");

                    b.ToTable("Teachers");
                });

            modelBuilder.Entity("SIMS.Teachers_Courses", b =>
                {
                    b.Property<int>("Teacher_ID")
                        .HasColumnType("int");

                    b.Property<int>("Course_ID")
                        .HasColumnType("int");

                    b.HasKey("Teacher_ID", "Course_ID");

                    b.HasIndex("Course_ID");

                    b.ToTable("Teachers_Courses");
                });

            modelBuilder.Entity("SIMS.Users", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<string>("Pass")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UserRole")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("ID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SIMS.AdminSystem", b =>
                {
                    b.HasOne("SIMS.Users", "User")
                        .WithOne()
                        .HasForeignKey("SIMS.AdminSystem", "Admin_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SIMS.Scores", b =>
                {
                    b.HasOne("SIMS.Courses", "Course")
                        .WithMany()
                        .HasForeignKey("Course_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SIMS.Courses", null)
                        .WithMany("Scores")
                        .HasForeignKey("CoursesCourse_ID");

                    b.HasOne("SIMS.Students", "Student")
                        .WithMany()
                        .HasForeignKey("Student_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SIMS.Students", null)
                        .WithMany("Scores")
                        .HasForeignKey("StudentsStudent_ID");

                    b.HasOne("SIMS.Teachers", "Teacher")
                        .WithMany()
                        .HasForeignKey("Teacher_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SIMS.Teachers", null)
                        .WithMany("Scores")
                        .HasForeignKey("TeachersTeacher_ID");

                    b.Navigation("Course");

                    b.Navigation("Student");

                    b.Navigation("Teacher");
                });

            modelBuilder.Entity("SIMS.Students_Courses", b =>
                {
                    b.HasOne("SIMS.Courses", "Course")
                        .WithMany("Students_Courses")
                        .HasForeignKey("Course_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SIMS.Students", "Student")
                        .WithMany("Students_Courses")
                        .HasForeignKey("Student_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("SIMS.Teachers_Courses", b =>
                {
                    b.HasOne("SIMS.Courses", "Course")
                        .WithMany("Teachers_Courses")
                        .HasForeignKey("Course_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SIMS.Teachers", "Teacher")
                        .WithMany("Teachers_Courses")
                        .HasForeignKey("Teacher_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Teacher");
                });

            modelBuilder.Entity("SIMS.Courses", b =>
                {
                    b.Navigation("Scores");

                    b.Navigation("Students_Courses");

                    b.Navigation("Teachers_Courses");
                });

            modelBuilder.Entity("SIMS.Students", b =>
                {
                    b.Navigation("Scores");

                    b.Navigation("Students_Courses");
                });

            modelBuilder.Entity("SIMS.Teachers", b =>
                {
                    b.Navigation("Scores");

                    b.Navigation("Teachers_Courses");
                });
#pragma warning restore 612, 618
        }
    }
}
