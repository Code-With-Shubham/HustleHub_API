using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HustleHub_API.BusinessLogic.Models.BusinessModels
{
    public class ProjectRequestVM
    {
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
    }
}
