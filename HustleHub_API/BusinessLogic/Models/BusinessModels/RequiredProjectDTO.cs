﻿namespace HustleHub_API.BusinessLogic.Models.BusinessModels
{
    public class RequiredProjectDTO
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? ProjectType { get; set; }
        public string? ComplexityLevel { get; set; }
        public string? Description { get; set; }

        // For returning document in Base64 format
        public string? ProjectDocsBase64 { get; set; }
        public IFormFile? ProjectDocs { get; set; } // File input from form-data

        public string? Mobile { get; set; }
        public decimal Budget { get; set; }
        public string? Tcstatus { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime UpdateAt { get; set; }
        public int? UpdateCount { get; set; }
    }

}
