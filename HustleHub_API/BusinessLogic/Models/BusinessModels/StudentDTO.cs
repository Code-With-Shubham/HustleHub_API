using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HustleHub_API.BusinessLogic.Models.BusinessModels
{
    public class StudentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Mobile { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public string? CollegeName { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool? Status { get; set; }
        public string? Course { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool? IsActive { get; set; }

        //To show data
        public String? ProfilePicBase64 { get; set; }
        public IFormFile? ProfilePic { get; set; } // File input from form-data

    }
}
