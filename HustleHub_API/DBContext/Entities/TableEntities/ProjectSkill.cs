using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HustleHub_API.DBContext.Entities.TableEntities;

public partial class ProjectSkill
{
    [Key]
    [Column("SkillID")]
    public int SkillId { get; set; }

    [Column("ProjectID")]
    public int ProjectId { get; set; }

    [StringLength(100)]
    public string? SkillName { get; set; }

    [ForeignKey("ProjectId")]
    [InverseProperty("ProjectSkills")]
    public virtual AdminProject Project { get; set; } = null!;
}
