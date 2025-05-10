using System.ComponentModel.DataAnnotations;

namespace HustleHub_API.BusinessLogic.Models.BusinessModels
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
