using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HustleHub_API.BusinessLogic.Models
{
    public class AdminProjectDTO
    {
        public int ProjectId { get; set; }

        public string? YoutubeLink { get; set; }

        public string? PreviewLink { get; set; }

        public string? ProjectDescription { get; set; }
        public string? Skill { get; set; }

        public decimal? BasePrice { get; set; }

        public decimal? PremiumPrice { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool? DisplayStatus { get; set; }
    }
}
