using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HustleHub_API.BusinessLogic.Models
{
    public class AdminProjectDTO
    {
        public int ProjectId { get; set; }
        public string? YoutubeLink { get; set; }
        public string? ProjectName { get; set; }
        public string? Description1 { get; set; }
        public string? LongDescription { get; set; }
        public string? Description2 { get; set; }
        public string? Category { get; set; }
        public string? LearningOutcomes { get; set; }
        public decimal? BasePrice { get; set; }
        public decimal? PremiumPrice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? DisplayStatus { get; set; }
        public byte[]? Image { get; set; }
        public List<string>? Skills { get; set; }
    }

}
