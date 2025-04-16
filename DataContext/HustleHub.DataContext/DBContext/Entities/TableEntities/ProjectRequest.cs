using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HustleHub.DataContext.DBContext.Entities.TableEntities;

[Table("ProjectRequest")]
public partial class ProjectRequest
{
    [Key]
    public int Id { get; set; }

    [StringLength(15)]
    public string Mobile { get; set; } = null!;

    [StringLength(100)]
    public string ProjectType { get; set; } = null!;

    [StringLength(50)]
    public string ProjectLevel { get; set; } = null!;

    public string ProjectDescription { get; set; } = null!;

    [StringLength(255)]
    public string? Documents { get; set; }

    [StringLength(50)]
    public string BudgetRange { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("Mobile")]
    [InverseProperty("ProjectRequests")]
    public virtual User MobileNavigation { get; set; } = null!;
}
