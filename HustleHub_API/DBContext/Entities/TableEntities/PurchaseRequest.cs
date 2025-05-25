using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HustleHub_API.DBContext.Entities.TableEntities;

[Table("PurchaseRequest")]
public partial class PurchaseRequest
{
    [Key]
    [Column("PurchaseID")]
    public int PurchaseId { get; set; }

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [Column("ProjectID")]
    public int ProjectId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PurchaseDate { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? IsPremium { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? IsStatus { get; set; }

    [ForeignKey("Email")]
    [InverseProperty("PurchaseRequests")]
    public virtual Student EmailNavigation { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("PurchaseRequests")]
    public virtual AdminProject Project { get; set; } = null!;
}
