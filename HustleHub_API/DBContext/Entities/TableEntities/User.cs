using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HustleHub_API.DBContext.Entities.TableEntities;

[Index("Mobile", Name = "UQ__Users__6FAE078260BFCD08", IsUnique = true)]
[Index("Email", Name = "UQ__Users__A9D105347B36EB0B", IsUnique = true)]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string? ProfilePic { get; set; }

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(15)]
    public string Mobile { get; set; } = null!;

    [StringLength(255)]
    public string Password { get; set; } = null!;

    [StringLength(255)]
    public string? College { get; set; }

    [StringLength(255)]
    public string? Address { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("MobileNavigation")]
    public virtual ICollection<ProjectRequest> ProjectRequests { get; set; } = new List<ProjectRequest>();
}
