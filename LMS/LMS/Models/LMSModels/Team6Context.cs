using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class Team6Context : DbContext
    {
        public virtual DbSet<ActiveProfessors> ActiveProfessors { get; set; }
        public virtual DbSet<Administrators> Administrators { get; set; }
        public virtual DbSet<Assignment> Assignment { get; set; }
        public virtual DbSet<AssignmentCat> AssignmentCat { get; set; }
        public virtual DbSet<Class> Class { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Enrolled> Enrolled { get; set; }
        public virtual DbSet<Professors> Professors { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<Submissions> Submissions { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=atr.eng.utah.edu;User Id=u0432850;Password=jelly;Database=Team6");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActiveProfessors>(entity =>
            {
                entity.HasKey(e => new { e.ProfessorId, e.Class });

                entity.HasIndex(e => e.Class)
                    .HasName("Class_idx");

                entity.Property(e => e.ProfessorId)
                    .HasColumnName("ProfessorID")
                    .HasMaxLength(8);

                entity.Property(e => e.Class).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Administrators>(entity =>
            {
                //entity.Property(e => e.DOB).HasColumnType("datetime");
                entity.HasKey(e => e.UId);

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasMaxLength(8);

                entity.HasOne(d => d.U)
                    .WithOne(p => p.Administrators)
                    .HasForeignKey<Administrators>(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("uID");
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.AssId);

                entity.Property(e => e.AssId)
                    .HasColumnName("AssID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AssignmentCat).HasColumnType("int(11)");

                entity.Property(e => e.Contents)
                    .IsRequired()
                    .HasColumnType("varchar(8192)");

                entity.Property(e => e.Due).HasColumnType("datetime(1)");

                entity.Property(e => e.Handin).HasColumnType("tinyint(4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Points).HasColumnType("int(11)");
            });

            modelBuilder.Entity<AssignmentCat>(entity =>
            {
                entity.HasKey(e => e.AcatId);

                entity.HasIndex(e => e.Class)
                    .HasName("Class_idx");

                entity.Property(e => e.AcatId)
                    .HasColumnName("ACatID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Class).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Weight).HasColumnType("int(11)");

                entity.HasOne(d => d.ClassNavigation)
                    .WithMany(p => p.AssignmentCat)
                    .HasForeignKey(d => d.Class)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ClassFK");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasIndex(e => e.Teacher)
                    .HasName("Professor_idx");
                entity.Property(e => e.Year).HasColumnType("int");

                entity.Property(e => e.ClassId)
                    .HasColumnName("ClassID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.End).HasColumnType("time");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Semester)
                    .IsRequired()
                    .HasMaxLength(6);

                entity.Property(e => e.Start).HasColumnType("time");

                entity.Property(e => e.Teacher)
                    .IsRequired()
                    .HasMaxLength(8);

                entity.HasOne(d => d.TeacherNavigation)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.Teacher)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professor");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => new { e.Department, e.Number });

                entity.HasIndex(e => e.CatId)
                    .HasName("CatID_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Department)
                    .HasName("Department_idx");
               
                entity.Property(e => e.Department).HasMaxLength(4);

                entity.Property(e => e.Number).HasColumnType("int(4)");

                entity.Property(e => e.CatId)
                    .HasColumnName("CatID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.Course)
                    .HasForeignKey(d => d.Department)
                    .HasConstraintName("Department");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Subject);

                entity.Property(e => e.Subject).HasMaxLength(4);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Enrolled>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.ClassId });

                entity.HasIndex(e => e.ClassId)
                    .HasName("ClassTaken_idx");

                entity.Property(e => e.StudentId)
                    .HasColumnName("StudentID")
                    .HasMaxLength(8);

                entity.Property(e => e.ClassId)
                    .HasColumnName("ClassID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Grade).HasMaxLength(2);

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrolled)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Class");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Enrolled)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("Student");
            });

            modelBuilder.Entity<Professors>(entity =>
            {
                entity.HasKey(e => e.UId);
                //entity.Property(e => e.DOB).HasColumnType("datetime");

                entity.HasIndex(e => e.Department)
                    .HasName("School_idx");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasMaxLength(8);

                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.Department)
                    .HasConstraintName("School");

                entity.HasOne(d => d.U)
                    .WithOne(p => p.Professors)
                    .HasForeignKey<Professors>(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID");
            });

            modelBuilder.Entity<Students>(entity =>
            {
                entity.HasKey(e => e.UId);

                entity.HasIndex(e => e.Major)
                    .HasName("Major_idx");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasMaxLength(8);

                entity.Property(e => e.Major)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.HasOne(d => d.MajorNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Major)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Major");

                entity.HasOne(d => d.U)
                    .WithOne(p => p.Students)
                    .HasForeignKey<Students>(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("studentID");
            });

            modelBuilder.Entity<Submissions>(entity =>
            {
                entity.HasKey(e => new { e.Assignment, e.Student });

                entity.HasIndex(e => e.Assignment)
                    .HasName("AssignmentID_idx");

                entity.HasIndex(e => e.Student)
                    .HasName("StudID_idx");

                entity.Property(e => e.Assignment).HasColumnType("int(11)");

                entity.Property(e => e.Student).HasMaxLength(8);

                entity.Property(e => e.ContentsBin).HasColumnType("blob");

                entity.Property(e => e.ContentsText).HasColumnType("varchar(8192)");

                entity.Property(e => e.Score).HasColumnType("int(11)");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.AssignmentNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.Assignment)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssignmentID");

                entity.HasOne(d => d.StudentNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.Student)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("StudID");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.Uid);
                entity.Property(e => e.Dob).HasColumnType("datetime");

                entity.HasIndex(e => e.Uid)
                    .HasName("uID_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Uid)
                    .HasColumnName("uID")
                    .HasMaxLength(8);

                entity.Property(e => e.First)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Last)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(8);
            });
        }
    }
}
