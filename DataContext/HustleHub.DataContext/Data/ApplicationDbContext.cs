using System;
using System.Collections.Generic;
using HustleHub.DataContext.DBContext.Entities.TableEntities;
using Microsoft.EntityFrameworkCore;

namespace HustleHub.DataContext.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ProjectRequest> ProjectRequests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=MANOHARES-5279\\SQLEXPRESS;Initial Catalog=HustelHubDB;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectR__3214EC0737F10813");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MobileNavigation).WithMany(p => p.ProjectRequests)
                .HasPrincipalKey(p => p.Mobile)
                .HasForeignKey(d => d.Mobile)
                .HasConstraintName("FK_ProjectRequest_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0741D229CA");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
