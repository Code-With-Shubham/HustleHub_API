using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HustleHub_API.DBContext.Entities.TableEntities;

[Table("StudentInfo")]
public partial class StudentInfo
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [Column("ProjectID")]
    public int? ProjectId { get; set; }

    [Column("RPID")]
    public int? Rpid { get; set; }

    [StringLength(250)]
    public string? Address { get; set; }

    public string? Courses { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("Email")]
    [InverseProperty("StudentInfos")]
    public virtual Student EmailNavigation { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("StudentInfos")]
    public virtual AdminProject? Project { get; set; }

    [ForeignKey("Rpid")]
    [InverseProperty("StudentInfos")]
    public virtual ProjectRequest? Rp { get; set; }
}
