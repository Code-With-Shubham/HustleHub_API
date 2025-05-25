using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub_API.BusinessLogic.Models;
using HustleHub_API.BusinessLogic.Models.APIResponse;
using HustleHub_API.BusinessLogic.Models.BusinessModels;
using HustleHub_API.BusinessLogic.Models.DTOs;
using HustleHub_API.DBContext.Entities.TableEntities;
using HustleHub_API.BusinessLogic.Models.DTOs;
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
        Task<LoginResponse> StudentLoginAsync(StudentLoginDTO model);
        Task<APIResponse> RegisterStudentAsync(StudentDTO model);
        Task<List<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByEmailAsync(string email);
        Task<Student?> GetStudentByIdAsync(int id);
        Task<APIResponse> DeleteStudentAsync(int id);
        Task<StudProjAPIResponse> ProjectRequestByIDAsync(int id);
        Task<IEnumerable<PurchaseRequestDto>> GetPurchaseRequestsByStudentIdAsync(int studentId);


        //Project Requirement   
        Task<APIResponse> SubmitProjectRequestAsync(RequiredProjectDTO model);
        Task<IEnumerable<ProjectRequest>> GetAllProjectsAsync();
        Task<ProjectRequest?> GetProjectByIdAsync(int id);

        //Admin Project
        Task<APIResponse> AddAdminProjectAsync(AdminProjectDTO model);
        Task<IEnumerable<AdminProjectDTO>> GetAllAdminProjectsAsync();
        Task<AdminProjectDTO?> GetAdminProjectByIdAsync(int id);
        Task<APIResponse> DeleteAdminProjectAsync(int projectId);

        Task<APIResponse> AddCategoryAsync(CategoryDTO model);
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<APIResponse> AdminLoginAsync(AdminLoginDTO model);

        //Purchase Requests
        Task<APIResponse> PurchaseRequestAsync(PurchaseRequestDto model);
        Task<IEnumerable<PurchaseRequest>> GetPurchaseRequestsAsync();
        Task<APIResponse> UpdatePurchaseRequestStatusAsync(int purchaseId, string isStatus);
        Task<APIResponse> SoftDeletePurchaseRequestAsync(int purchaseId);
    }
}
