using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub_API.BusinessLogic.Models;
using HustleHub_API.BusinessLogic.Models.BusinessModels;
using HustleHub_API.DBContext.Entities.TableEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HustleHub.BusinessArea.Interface
{
    public interface IRepository
    {
        //Student Registration
        Task<APIResponse> RegisterStudentAsync(Students model, IFormFile? profilePicFile);
        Task<List<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByEmailAsync(string email);

        //Project Requirement   
        Task<APIResponse> SubmitProjectRequestAsync(Projects model, IFormFile? projectDocFile);
        Task<IEnumerable<ProjectRequest>> GetAllProjectsAsync();
        Task<ProjectRequest?> GetProjectByIdAsync(int id);

        //Admin Project
        Task<APIResponse> AddAdminProjectAsync(AdminProjectDTO model, IFormFile? ProjectIconImage);
        Task<IEnumerable<AdminProjectDTO>> GetAllAdminProjectsAsync();
        Task<AdminProjectDTO?> GetAdminProjectByIdAsync(int id);



    }
}
