using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HustleHub_API.DBContext.Entities.TableEntities;

[Table("CareerPath")]
public partial class CareerPath
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(100)]
    public string Designation { get; set; } = null!;

    public string? Description { get; set; }

    public string? Skills { get; set; }
}
