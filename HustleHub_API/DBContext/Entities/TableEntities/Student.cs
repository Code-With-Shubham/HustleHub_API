using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HustleHub_API.DBContext.Entities.TableEntities;

[Table("Student")]
public partial class Student
{
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(15)]
    public string Mobile { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [StringLength(150)]
    public string? CollegeName { get; set; }

    [Key]
    [StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(100)]
    public string Password { get; set; } = null!;

    public bool? Status { get; set; }

    [StringLength(100)]
    public string? Course { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateAt { get; set; }

    public string? ProfilePic { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastLoginAt { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("EmailNavigation")]
    public virtual ICollection<ProjectRequest> ProjectRequests { get; set; } = new List<ProjectRequest>();

    [InverseProperty("EmailNavigation")]
    public virtual ICollection<StudentInfo> StudentInfos { get; set; } = new List<StudentInfo>();
}
