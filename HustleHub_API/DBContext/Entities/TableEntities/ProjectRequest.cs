using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HustleHub_API.DBContext.Entities.TableEntities;

[Table("ProjectRequest")]
public partial class ProjectRequest
{
    [Key]
    [Column("RPID")]
    public int Rpid { get; set; }

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(100)]
    public string? ProjectType { get; set; }

    [StringLength(50)]
    public string? ComplexityLevel { get; set; }

    public string? Description { get; set; }

    public string? ProjectDocs { get; set; }

    [StringLength(15)]
    public string? Mobile { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Budget { get; set; }

    [Column("TCStatus")]
    [StringLength(50)]
    public string? Tcstatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RequestDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public int? UpdateCount { get; set; }

    public bool? Status { get; set; }

    [StringLength(150)]
    public string? ApprovedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ApprovedDate { get; set; }

    [ForeignKey("Email")]
    [InverseProperty("ProjectRequests")]
    public virtual Student EmailNavigation { get; set; } = null!;

    [InverseProperty("Rp")]
    public virtual ICollection<StudentInfo> StudentInfos { get; set; } = new List<StudentInfo>();
}
