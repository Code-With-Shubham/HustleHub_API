using System;
using System.Collections.Generic;
using HustleHub_API.DBContext.Entities.TableEntities;
using Microsoft.EntityFrameworkCore;

namespace HustleHub_API.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminLogin> AdminLogins { get; set; }

    public virtual DbSet<AdminProject> AdminProjects { get; set; }

    public virtual DbSet<CareerPath> CareerPaths { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ProjectRequest> ProjectRequests { get; set; }

    public virtual DbSet<ProjectSkill> ProjectSkills { get; set; }

    public virtual DbSet<PurchaseRequest> PurchaseRequests { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentInfo> StudentInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=hustlehub.cfw6scimuwug.ap-south-1.rds.amazonaws.com,1433;Database=hustlehubdb;User Id=hustlehub;Password=Manohares;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminLogin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__AdminLog__719FE4E8D5E78344");
        });

        modelBuilder.Entity<AdminProject>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__AdminPro__761ABED03E132434");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DisplayStatus).HasDefaultValue(true);
        });

        modelBuilder.Entity<CareerPath>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CareerPa__3214EC27AF695E04");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2B9C3024FB");
        });

        modelBuilder.Entity<ProjectRequest>(entity =>
        {
            entity.HasKey(e => e.Rpid).HasName("PK__ProjectR__4484A8331234398A");

            entity.Property(e => e.RequestDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UpdateCount).HasDefaultValue(0);

            entity.HasOne(d => d.EmailNavigation).WithMany(p => p.ProjectRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectRequest_Student");
        });

        modelBuilder.Entity<ProjectSkill>(entity =>
        {
            entity.HasKey(e => e.SkillId).HasName("PK__ProjectS__DFA091E72CDD92AE");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectSkills).HasConstraintName("FK_ProjectSkills_AdminProject");
        });

        modelBuilder.Entity<PurchaseRequest>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK__Purchase__6B0A6BDE26EC7782");

            entity.Property(e => e.PurchaseDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.EmailNavigation).WithMany(p => p.PurchaseRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentPurchase_Student");

            entity.HasOne(d => d.Project).WithMany(p => p.PurchaseRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentPurchase_AdminProject");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Email).HasName("PK__Student__A9D10535300DE5A6");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<StudentInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StudentI__3214EC2703586849");

            entity.HasOne(d => d.EmailNavigation).WithMany(p => p.StudentInfos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentInfo_Student");

            entity.HasOne(d => d.Project).WithMany(p => p.StudentInfos).HasConstraintName("FK_StudentInfo_AdminProject");

            entity.HasOne(d => d.Rp).WithMany(p => p.StudentInfos).HasConstraintName("FK_StudentInfo_ProjectRequest");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
