using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub.BusinessArea.Models.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HustleHub.BusinessArea.Interface
{
    public interface IRepository
    {
        Task<APIResponse> RegisterUserAsync(Users model);
        Task<APIResponse> RequiredProjectAsync(ProjectReq model);
    }
}
