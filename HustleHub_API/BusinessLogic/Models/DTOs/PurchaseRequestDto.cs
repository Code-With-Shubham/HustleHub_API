namespace HustleHub_API.BusinessLogic.Models.DTOs
{
    public class PurchaseRequestDto
    {
        public int PurchaseId { get; set; }
        public string Email { get; set; }
        public int ProjectId { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public String? IsPremium { get; set; }
        public String? IsBasic { get; set; }
        public string? IsStatus { get; set; }
    }
}