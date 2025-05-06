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
            var students = await _dbcontext.Students.ToListAsync();

            foreach (var student in students)
            {
                if (!string.IsNullOrEmpty(student.ProfilePic))
                {
                    var imagePath = Path.Combine(_environment.ContentRootPath, "Uploads", "Images", student.ProfilePic);
                    if (File.Exists(imagePath))
                    {
                        // Convert the image to a Base64 string
                        var imageBytes = await File.ReadAllBytesAsync(imagePath);
                        student.ProfilePic = Convert.ToBase64String(imageBytes);
                    }
                    else
                    {
                        student.ProfilePic = null; // If the file doesn't exist, set it to null
                    }
                }
            }

            return students;
        }


        public async Task<Student?> GetStudentByEmailAsync(string email)
        {
            return await _dbcontext.Students.FirstOrDefaultAsync(s => s.Email == email);
        }


        //Project Requirement

        public async Task<APIResponse> SubmitProjectRequestAsync(Projects model, IFormFile? projectDocFile)
        {
            try
            {
                if (model.Email == null)
                {
                    return new APIResponse { Code = 400, Message = "Email cannot be null", Status = "error" };
                }
                if (projectDocFile != null && projectDocFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    var filePath = Path.Combine(uploadsFolder, projectDocFile.FileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await projectDocFile.CopyToAsync(stream);

                    model.ProjectDocsFile = projectDocFile.FileName;
                }

                ProjectRequest project = new ProjectRequest
                {
                    Rpid = model.Id,
                    Email = model.Email,
                    ProjectType = model.ProjectType,
                    ComplexityLevel = model.ComplexityLevel,
                    Description = model.Description,
                    ProjectDocs = model.ProjectDocsFile,
                    Mobile = model.Mobile,
                    Budget = model.Budget,
                    Tcstatus = model.Tcstatus,
                    ApprovedBy = model.ApprovedBy,
                    ApprovedDate = model.ApprovedDate,
                    UpdateDate = DateTime.UtcNow,
                };

                _dbcontext.ProjectRequests.Add(project);
                await _dbcontext.SaveChangesAsync();

                return new APIResponse { Code = 200, Message = "Project request submitted successfully", Status="success" };
            }
            catch (Exception ex)
            {
                return new APIResponse { Code = 500, Message = "Error: " + ex.Message, Status = "error" };
            }
        }


        public async Task<IEnumerable<ProjectRequest>> GetAllProjectsAsync()
        {
            var projects = await _dbcontext.ProjectRequests
                                           .OrderByDescending(p => p.UpdateDate)
                                           .ToListAsync();

            foreach (var project in projects)
            {
                if (!string.IsNullOrEmpty(project.ProjectDocs))
                {
                    var documentPath = Path.Combine(_environment.ContentRootPath, "Uploads", "Documents", project.ProjectDocs);
                    if (File.Exists(documentPath))
                    {
                        // Convert the document to a Base64 string
                        var documentBytes = await File.ReadAllBytesAsync(documentPath);
                        project.ProjectDocs = Convert.ToBase64String(documentBytes);
                    }
                    else
                    {
                        project.ProjectDocs = null; // If the file doesn't exist, set it to null
                    }
                }
            }

            return projects;
        }



    }
}
