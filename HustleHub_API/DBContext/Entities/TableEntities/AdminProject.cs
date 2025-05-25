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

    [StringLength(255)]
    public string? ProjectName { get; set; }

    public string? Description1 { get; set; }

    public string? LongDescription { get; set; }

    public string? Description2 { get; set; }

    public string? Image { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    public string? LearningOutcomes { get; set; }

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
    public virtual ICollection<ProjectSkill> ProjectSkills { get; set; } = new List<ProjectSkill>();

    [InverseProperty("Project")]
    public virtual ICollection<PurchaseRequest> PurchaseRequests { get; set; } = new List<PurchaseRequest>();

    [InverseProperty("Project")]
    public virtual ICollection<StudentInfo> StudentInfos { get; set; } = new List<StudentInfo>();
}
