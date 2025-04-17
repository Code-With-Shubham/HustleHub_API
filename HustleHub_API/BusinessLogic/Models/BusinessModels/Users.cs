using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HustleHub.BusinessArea.Models.BusinessModels
{
    public class Users
    {

        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(255)]
        public string? ProfilePic { get; set; }

        [StringLength(100)]
        public string Email { get; set; } = null!;

        [StringLength(15)]
        public string Mobile { get; set; } = null!;

        [StringLength(255)]
        public string Password { get; set; } = null!;

        [StringLength(255)]
        public string? College { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
    }
}
