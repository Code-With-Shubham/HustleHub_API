using HustleHub.BusinessArea.Interface;
using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub.BusinessArea.Models.BusinessModels;
using HustleHub.DataContext.Data;
using HustleHub.DataContext.DBContext.Entities.TableEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HustleHub.BusinessArea.Repository
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly HttpClient _httpClient;
        public Repository(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
            _httpClient = new HttpClient();
        }


        public async Task<APIResponse> RegisterUserAsync(Users model)
        {
            APIResponse result = new APIResponse();

            try
            {

                User obj = new User
                {
                   // Id = model.Id,
                    Name = model.Name,
                    ProfilePic = model.ProfilePic,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Password = model.Password,
                    College = model.College,
                    Address = model.Address,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _dbcontext.Users.Add(obj);
                int cnt = await _dbcontext.SaveChangesAsync();
                if (cnt > 0)
                {
                    result.Code = 200;
                    result.Status = "success";
                    result.Message = "Registration Successfully";
                }
                else
                {
                    result.Code = 500;
                    result.Status = "error";
                    result.Message = "Failed to save data.";
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Status = "error";
                result.Message = ex.Message;
                return result;
            }

        }


        public async Task<APIResponse> RequiredProjectAsync(ProjectReq model)
        {
            APIResponse result = new APIResponse();

            try
            {
                // Check if mobile exists in Users table
                var userExists = await _dbcontext.Users.AnyAsync(u => u.Mobile == model.Mobile);
                if (!userExists)
                {
                    result.Code = 400;
                    result.Status = "error";
                    result.Message = "User with this mobile number does not exist.";
                    return result;
                }

                // Proceed to save the project
                ProjectRequest obj = new ProjectRequest
                {
                    Id = model.Id,
                    Mobile = model.Mobile,
                    ProjectType = model.ProjectType,
                    ProjectLevel = model.ProjectLevel,
                    ProjectDescription = model.ProjectDescription,
                    Documents = model.Documents,
                    BudgetRange = model.BudgetRange,
                    CreatedAt = model.CreatedAt,
                    UpdatedAt = model.UpdatedAt
                };

                _dbcontext.ProjectRequests.Add(obj);
                int cnt = await _dbcontext.SaveChangesAsync();

                if (cnt > 0)
                {
                    result.Code = 200;
                    result.Status = "success";
                    result.Message = "Project Saved Successfully";
                }
                else
                {
                    result.Code = 500;
                    result.Status = "error";
                    result.Message = "Failed to save data.";
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Status = "error";
                result.Message = ex.InnerException?.Message ?? ex.Message;
                return result;
            }
        }


    }
}
