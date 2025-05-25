using HustleHub_API.BusinessLogic.Models.DTOs;

public class PurchaseRequestWithStudentDto
{
    // PurchaseRequest fields
    public int PurchaseId { get; set; }
    public string Email { get; set; }
    public int ProjectId { get; set; } // Add this property to fix CS0117
    public DateTime? PurchaseDate { get; set; }
    public string? IsPremium { get; set; }
    public string? IsStatus { get; set; }

    // Student fields
    public int StudentId { get; set; }
    public string Name { get; set; }
    public string Mobile { get; set; }
}