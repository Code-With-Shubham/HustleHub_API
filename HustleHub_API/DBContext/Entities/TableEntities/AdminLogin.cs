using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HustleHub_API.DBContext.Entities.TableEntities;

[Index("Email", Name = "UQ__AdminLog__A9D10534DF4F7C87", IsUnique = true)]
public partial class AdminLogin
{
    [Key]
    [Column("AdminID")]
    public int AdminId { get; set; }

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(100)]
    public string Password { get; set; } = null!;

    [StringLength(50)]
    public string? Type { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastLoginAt { get; set; }
}
