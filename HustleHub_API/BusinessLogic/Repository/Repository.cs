using Azure;
using HustleHub.BusinessArea.Interface;
using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub_API.BusinessLogic.Models.BusinessModels;
using HustleHub_API.Data;
using HustleHub_API.DBContext.Entities.TableEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
        private readonly IWebHostEnvironment _environment;
        public Repository(ApplicationDbContext dbcontext, IWebHostEnvironment environment)
        {
            _dbcontext = dbcontext;
            _httpClient = new HttpClient();
            _environment = environment;
        }


        public async Task<APIResponse> RegisterStudentAsync(Students model, IFormFile? profilePicFile)
        {
            APIResponse result = new APIResponse();

            try
            {
                string? profilePicFileName = null;

                // Upload Profile Pic if available
                if (profilePicFile != null && profilePicFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads", "Images");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    profilePicFileName = Guid.NewGuid().ToString() + Path.GetExtension(profilePicFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, profilePicFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicFile.CopyToAsync(stream);
                    }
                }

                Student obj = new Student
                {
                    Name = model.Name,
                    ProfilePic = profilePicFileName,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Password = model.Password,
                    CollegeName = model.CollegeName,
                    CreatedAt = DateTime.Now,
                    UpdateAt = null,
                    IsActive = true,
                    Status = true
                };

                _dbcontext.Students.Add(obj);
                int cnt = await _dbcontext.SaveChangesAsync();

                if (cnt > 0)
                {
                    result.Code = 200;
                    result.Status = "success";
                    result.Message = "Student Registered Successfully.";
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

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _dbcontext.Students.ToListAsync();
        }

        public async Task<Student?> GetStudentByEmailAsync(string email)
        {
            return await _dbcontext.Students.FirstOrDefaultAsync(s => s.Email == email);
        }




        // Project

        // Get all ProjectRequests
        public async Task<IEnumerable<ProjectRequest>> GetProjectRequestsAsync()
        {
            return await _dbcontext.ProjectRequests.ToListAsync();
        }

        // Get a specific ProjectRequest by its ID
        public async Task<ProjectRequest> GetProjectRequestByIdAsync(int id)
        {
            return await _dbcontext.ProjectRequests
                .Include(pr => pr.EmailNavigation)  // If you want to include related Student data
                .FirstOrDefaultAsync(pr => pr.Rpid == id);
        }

        // Create a new ProjectRequest
        // Create a new ProjectRequest
        public async Task<APIResponse> CreateProjectRequestAsync(ProjectRequestVM model, IFormFile projectDocsFile)
        {
            APIResponse response = new();
            try
            {
                string projectDocsFileName = null;

                // Handle file upload if projectDocsFile is provided
                if (projectDocsFile != null && projectDocsFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads", "ProjectDocs");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    projectDocsFileName = Guid.NewGuid().ToString() + Path.GetExtension(projectDocsFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, projectDocsFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await projectDocsFile.CopyToAsync(stream);
                    }
                }

                // Create a new ProjectRequest object and map the model data
                var projectRequest = new ProjectRequest
                {
                    Email = model.Email,
                    ProjectType = model.ProjectType,
                    ComplexityLevel = model.ComplexityLevel,
                    Description = model.Description,
                    ProjectDocs = projectDocsFileName,
                    Mobile = model.Mobile,
                    Budget = model.Budget,
                    Tcstatus = model.Tcstatus,
                    RequestDate = DateTime.Now,
                    Status = true,
                    ApprovedBy = model.ApprovedBy,
                    ApprovedDate = model.ApprovedDate
                };

                // Add the ProjectRequest to the database
                await _dbcontext.ProjectRequests.AddAsync(projectRequest);
                int cnt = await _dbcontext.SaveChangesAsync();

                // Check if the ProjectRequest was successfully saved
                if (cnt > 0)
                {
                    response.Code = 200;
                    response.Status = "success";
                    response.Message = "Project Request Created Successfully.";
                }
                else
                {
                    response.Code = 500;
                    response.Status = "error";
                    response.Message = "Failed to save data.";
                }
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Status = "error";
                response.Message = ex.Message;
            }

            return response;
        }

        // Get ProjectRequest by Email
       
    }
}
