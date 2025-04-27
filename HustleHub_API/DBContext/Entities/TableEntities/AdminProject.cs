using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HustleHub_API.DBContext.Entities.TableEntities;

[Table("AdminProject")]
public partial class AdminProject
{
    [Key]
    [Column("ProjectID")]
    public int ProjectId { get; set; }

    public string? YoutubeLink { get; set; }

    public string? PreviewLink { get; set; }

    public string? ProjectDescription { get; set; }

    [StringLength(200)]
    public string? Skill { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? BasePrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PremiumPrice { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    public bool? DisplayStatus { get; set; }

    [InverseProperty("Project")]
    public virtual ICollection<StudentInfo> StudentInfos { get; set; } = new List<StudentInfo>();
}
