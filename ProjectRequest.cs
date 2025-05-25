public class ProjectRequest
{
    public int Rpid { get; set; }
    public string Email { get; set; }
    public string? ProjectType { get; set; }
    public string? ComplexityLevel { get; set; }
    public string? Description { get; set; }
    public string? ProjectDocs { get; set; }
    public string? Mobile { get; set; }
    public decimal? Budget { get; set; }
    public string? Tcstatus { get; set; }
    public DateTime? RequestDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public int? UpdateCount { get; set; }
    public bool? Status { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public virtual Student EmailNavigation { get; set; }
    public virtual ICollection<StudentInfo> StudentInfos { get; set; }

    // Add the missing property to fix CS1061
    public string? ProjectDocsBase64 { get; set; }
}
