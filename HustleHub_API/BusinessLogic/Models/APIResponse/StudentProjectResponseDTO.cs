using HustleHub_API.BusinessLogic.Models.BusinessModels;
using HustleHub_API.DBContext.Entities.TableEntities;


public class StudentProjectResponseDTO
{
    public StudentDTO Student { get; set; }
    public List<RequiredProjectDTO> ProjectRequests { get; set; }
}
