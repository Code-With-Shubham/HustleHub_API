public class RequiredProjectDTO
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? ProjectType { get; set; }
    public string? ComplexityLevel { get; set; }
    public string? Description { get; set; }
    public string? ProjectDocsFile { get; set; }
    public string? Mobile { get; set; }
    public decimal Budget { get; set; }
    public string? Tcstatus { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime UpdateAt { get; set; }
    public int? UpdateCount { get; set; }
}
