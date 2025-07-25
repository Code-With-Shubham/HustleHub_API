﻿using HustleHub.BusinessArea.Interface;
using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub_API.BusinessLogic.Models;
using HustleHub_API.BusinessLogic.Models.APIResponse;
using HustleHub_API.BusinessLogic.Models.BusinessModels;
using HustleHub_API.BusinessLogic.Models.DTOs;
using HustleHub_API.Data;
using HustleHub_API.DBContext.Entities.TableEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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
        private readonly EmailService _emailService;
        public Repository(ApplicationDbContext dbcontext, IWebHostEnvironment environment, EmailService emailService)
        {
            _dbcontext = dbcontext;
            _httpClient = new HttpClient();
            _environment = environment;
            _emailService = emailService;
        }

        public async Task<LoginResponse> StudentLoginAsync(StudentLoginDTO model)
        {
            try
            {
                var student = await _dbcontext.Students
                    .FirstOrDefaultAsync(s => s.Email == model.Email && s.Password == model.Password);

                if (student == null)
                {
                    return new LoginResponse
                    {
                        Code = 401,
                        Status = "error",
                        Message = "Invalid email or password.",
                        Data = null
                    };
                }

                return new LoginResponse
                {
                    Code = 200,
                    Status = "success",
                    Message = "Login successful.",
                    Data = student // ✅ Important: return student object
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Code = 500,
                    Status = "error",
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }


        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            try
            {
                // Fetch the student by ID
                var student = await _dbcontext.Students.FirstOrDefaultAsync(s => s.Id == id);
                return student;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching student by ID: {ex.Message}");
                return null;
            }
        }

        public async Task<APIResponse> DeleteStudentAsync(int id)
        {
            try
            {
                // Find the student by ID
                var student = await _dbcontext.Students.FirstOrDefaultAsync(s => s.Id == id);


                if (student == null)
                {
                    return new APIResponse
                    {
                        Code = 404,
                        Status = "error",
                        Message = "Student not found."
                    };
                }

                var pr = await _dbcontext.ProjectRequests.FirstOrDefaultAsync(x => x.Email == student.Email);

                if (student.Email == pr.Email)
                {
                    _dbcontext.ProjectRequests.Remove(pr);
                    await _dbcontext.SaveChangesAsync();
                }

                // Remove the student
                _dbcontext.Students.Remove(student);
                await _dbcontext.SaveChangesAsync();


                return new APIResponse
                {
                    Code = 200,
                    Status = "success",
                    Message = "Student deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    Code = 500,
                    Status = "error",
                    Message = $"Error: {ex.Message}"
                };
            }
        }


        public async Task<APIResponse> RegisterStudentAsync(StudentDTO model)
        {
            APIResponse result = new APIResponse();
            try
            {
                byte[]? profilePicBytes = null;

                // Handle image from form-data
                if (model.ProfilePic != null && model.ProfilePic.Length > 0)
                {
                    if (model.ProfilePic.Length > 2 * 1024 * 1024)
                    {
                        return new APIResponse
                        {
                            Code = 400,
                            Status = "error",
                            Message = "Profile picture size must be less than 2MB."
                        };
                    }

                    using (var ms = new MemoryStream())
                    {
                        await model.ProfilePic.CopyToAsync(ms);
                        profilePicBytes = ms.ToArray();
                    }
                }

                Student obj = new Student
                {
                    Name = model.Name,
                    ProfilePic = profilePicBytes,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Password = model.Password,
                    CollegeName = model.CollegeName,
                    CreatedAt = DateTime.Now,
                    UpdateAt = null,
                    IsActive = true,
                    Status = true,
                    Course = model.Course,
                    LastLoginAt = model.LastLoginAt
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
                if (student.ProfilePic != null && student.ProfilePic.Length > 0)
                {
                    student.ProfilePic = student.ProfilePic; // Already byte[]
                }
            }

            return students;
        }
        public async Task<Student?> GetStudentByEmailAsync(string email)
        {
            return await _dbcontext.Students.FirstOrDefaultAsync(s => s.Email == email);
        }


        // Fix for CS1061: Replace the incorrect usage of CopyToAsync on a byte[] with the correct logic.

        public async Task<IEnumerable<ProjectRequest>> GetAllProjectsAsync()
        {
            var projects = await _dbcontext.ProjectRequests.OrderByDescending(p => p.UpdateDate).ToListAsync();
            return projects;
        }

        public async Task<APIResponse> SubmitProjectRequestAsync([FromForm] RequiredProjectDTO model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    return new APIResponse { Code = 400, Message = "Email cannot be null", Status = "error" };
                }

                var mailcheck = await _dbcontext.Students
                    .FirstOrDefaultAsync(x => x.Email == model.Email || x.Mobile == model.Mobile);

                if (mailcheck == null)
                {
                    return new APIResponse { Code = 400, Message = "This email id is not registered", Status = "error" };
                }

                byte[]? projectDocBytes = null;

                if (model.ProjectDocs != null && model.ProjectDocs.Length > 0)
                {
                    if (model.ProjectDocs.Length > 2 * 1024 * 1024) // 2MB
                    {
                        return new APIResponse
                        {
                            Code = 400,
                            Message = "Project document size must be less than 2MB.",
                            Status = "error"
                        };
                    }

                    using (var ms = new MemoryStream())
                    {
                        await model.ProjectDocs.CopyToAsync(ms);
                        projectDocBytes = ms.ToArray();
                    }
                }

                var project = new ProjectRequest
                {
                    Email = model.Email,
                    ProjectType = model.ProjectType,
                    ComplexityLevel = model.ComplexityLevel,
                    Description = model.Description,
                    ProjectDocs = projectDocBytes,
                    Mobile = model.Mobile,
                    Budget = model.Budget,
                    Tcstatus = model.Tcstatus,
                    ApprovedBy = model.ApprovedBy,
                    ApprovedDate = model.ApprovedDate,
                    RequestDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    UpdateCount = model.UpdateCount,
                    Status = true
                };

                _dbcontext.ProjectRequests.Add(project);
                await _dbcontext.SaveChangesAsync();

                return new APIResponse
                {
                    Code = 200,
                    Message = "Project request submitted successfully",
                    Status = "success"
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    Code = 500,
                    Message = $"Error: {ex.Message}",
                    Status = "error"
                };
            }
        }




        public async Task<StudProjAPIResponse> ProjectRequestByIDAsync(int id)
        {
            try
            {
                var student = await _dbcontext.Students
                    .Where(s => s.Id == id)
                    .Select(s => new StudentDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Email = s.Email
                    })
                    .FirstOrDefaultAsync();

                if (student == null)
                {
                    return new StudProjAPIResponse
                    {
                        Code = 404,
                        Status = "error",
                        Message = "Student not found.",
                        Data = null
                    };
                }

                var projects = await _dbcontext.ProjectRequests
                    .Where(p => p.Email == student.Email)
                    .Select(p => new RequiredProjectDTO
                    {
                        Id = p.Rpid,
                        Email = p.Email,
                        ProjectType = p.ProjectType,
                        ComplexityLevel = p.ComplexityLevel,
                        Description = p.Description,

                        // ✅ Convert varbinary (byte[]) to Base64 string for response
                        ProjectDocsBase64 = p.ProjectDocs != null ? Convert.ToBase64String(p.ProjectDocs) : null,

                        Mobile = p.Mobile,
                        Budget = p.Budget ?? 0m,
                        ApprovedBy = p.ApprovedBy,
                        ApprovedDate = p.ApprovedDate,
                        UpdateAt = p.UpdateDate ?? DateTime.MinValue,
                        UpdateCount = p.UpdateCount
                    })
                    .ToListAsync();

                return new StudProjAPIResponse
                {
                    Code = 200,
                    Status = "success",
                    Message = "Project requests retrieved successfully.",
                    Data = new StudentProjectResponseDTO
                    {
                        Student = student,
                        ProjectRequests = projects
                    }
                };
            }
            catch (Exception ex)
            {
                return new StudProjAPIResponse
                {
                    Code = 500,
                    Status = "error",
                    Message = $"Internal Server Error: {ex.Message}",
                    Data = null
                };
            }
        }






        //Admin Project
        public async Task<APIResponse> AddAdminProjectAsync(AdminProjectDTO dto)
        {
            var strategy = _dbcontext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _dbcontext.Database.BeginTransactionAsync();

                try
                {
                    byte[]? imageBytes = null;

                    if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await dto.ImageFile.CopyToAsync(ms);
                            imageBytes = ms.ToArray();
                        }
                    }

                    var project = new AdminProject
                    {
                        ProjectName = dto.ProjectName,
                        YoutubeLink = dto.YoutubeLink,
                        Description1 = dto.Description1,
                        LongDescription = dto.LongDescription,
                        Description2 = dto.Description2,
                        CategoryId = dto.Category,
                        LearningOutcomes = dto.LearningOutcomes,
                        BasePrice = dto.BasePrice,
                        PremiumPrice = dto.PremiumPrice,
                        CreatedAt = DateTime.UtcNow,
                        DisplayStatus = dto.DisplayStatus,
                        Image = imageBytes // ✅ save to database
                    };

                    _dbcontext.AdminProjects.Add(project);
                    await _dbcontext.SaveChangesAsync();

                    if (dto.Skills != null && dto.Skills.Any())
                    {
                        var projectSkills = dto.Skills.Select(skill => new ProjectSkill
                        {
                            ProjectId = project.ProjectId,
                            SkillName = skill
                        });

                        _dbcontext.ProjectSkills.AddRange(projectSkills);
                        await _dbcontext.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();

                    return new APIResponse
                    {
                        Code = 200,
                        Status = "success",
                        Message = "Admin project and skills added successfully."
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new APIResponse
                    {
                        Code = 500,
                        Status = "error",
                        Message = $"Error: {ex.Message}"
                    };
                }
            });
        }



        public async Task<IEnumerable<AdminProjectDTO>> GetAllAdminProjectsAsync()
        {
            var projects = await _dbcontext.AdminProjects
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var response = new List<AdminProjectDTO>();

            foreach (var project in projects)
            {
                string? imageBase64 = null;

                if (project.Image != null && project.Image.Length > 0)
                {
                    imageBase64 = "data:image/png;base64," + Convert.ToBase64String(project.Image);
                }

                var skills = await _dbcontext.ProjectSkills
                    .Where(s => s.ProjectId == project.ProjectId)
                    .Select(s => s.SkillName)
                    .ToListAsync();

                response.Add(new AdminProjectDTO
                {
                    ProjectId = project.ProjectId,
                    YoutubeLink = project.YoutubeLink,
                    ProjectName = project.ProjectName,
                    Description1 = project.Description1,
                    LongDescription = project.LongDescription,
                    Description2 = project.Description2,
                    Category = project.CategoryId,
                    LearningOutcomes = project.LearningOutcomes,
                    BasePrice = project.BasePrice,
                    PremiumPrice = project.PremiumPrice,
                    CreatedAt = project.CreatedAt,
                    UpdatedAt = project.UpdatedAt,
                    DisplayStatus = project.DisplayStatus,
                    Image = imageBase64, // ✅ for display
                    Skills = skills
                });
            }

            return response;
        }


        public async Task<AdminProjectDTO?> GetAdminProjectByIdAsync(int id)
        {
            var project = await _dbcontext.AdminProjects.FirstOrDefaultAsync(p => p.ProjectId == id);

            if (project == null)
                return null;

            string? imageBase64 = null;

            if (project.Image != null && project.Image.Length > 0)
            {
                imageBase64 = "data:image/png;base64," + Convert.ToBase64String(project.Image);
            }

            var skills = await _dbcontext.ProjectSkills
                .Where(s => s.ProjectId == project.ProjectId)
                .Select(s => s.SkillName)
                .ToListAsync();

            return new AdminProjectDTO
            {
                ProjectId = project.ProjectId,
                YoutubeLink = project.YoutubeLink,
                ProjectName = project.ProjectName,
                Description1 = project.Description1,
                LongDescription = project.LongDescription,
                Description2 = project.Description2,
                Category = project.CategoryId,
                LearningOutcomes = project.LearningOutcomes,
                BasePrice = project.BasePrice,
                PremiumPrice = project.PremiumPrice,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                DisplayStatus = project.DisplayStatus,
                Image = imageBase64, // ✅ for display
                Skills = skills
            };
        }



        public async Task<APIResponse> DeleteAdminProjectAsync(int projectId)
        {
            try
            {
                // Step 1: Fetch the project including related ProjectSkills
                var project = await _dbcontext.AdminProjects
                    .Include(p => p.ProjectSkills)
                    .FirstOrDefaultAsync(p => p.ProjectId == projectId);

                if (project == null)
                {
                    return new APIResponse
                    {
                        Code = 404,
                        Status = "error",
                        Message = "Admin project not found."
                    };
                }

                // Step 2: Delete related ProjectSkills (if any)
                if (project.ProjectSkills != null && project.ProjectSkills.Any())
                {
                    _dbcontext.ProjectSkills.RemoveRange(project.ProjectSkills);
                }

                // Step 3: Delete related PurchaseRequests (if any)
                var relatedPurchases = await _dbcontext.PurchaseRequests
                    .Where(p => p.ProjectId == projectId)
                    .ToListAsync();

                if (relatedPurchases.Any())
                {
                    _dbcontext.PurchaseRequests.RemoveRange(relatedPurchases);
                }

                // Step 4: Delete the AdminProject
                _dbcontext.AdminProjects.Remove(project);

                // Step 5: Save all changes
                int cnt = await _dbcontext.SaveChangesAsync();

                return new APIResponse
                {
                    Code = cnt > 0 ? 200 : 500,
                    Status = cnt > 0 ? "success" : "error",
                    Message = cnt > 0 ? "Admin project and all related data deleted successfully." : "Deletion failed."
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    Code = 500,
                    Status = "error",
                    Message = $"Exception: {ex.Message}" +
                              (ex.InnerException != null ? $". Inner: {ex.InnerException.Message}" : "")
                };
            }
        }





        public async Task<APIResponse> AddCategoryAsync(CategoryDTO model)
        {
            try
            {
                // Check if the category already exists
                var existingCategory = await _dbcontext.Categories
                    .FirstOrDefaultAsync(c => c.CategoryName == model.CategoryName);

                if (existingCategory != null)
                {
                    return new APIResponse
                    {
                        Code = 400,
                        Status = "error",
                        Message = "Category already exists."
                    };
                }

                // Add the new category
                var category = new Category
                {
                    CategoryName = model.CategoryName,
                };

                _dbcontext.Categories.Add(category);
                await _dbcontext.SaveChangesAsync();

                return new APIResponse
                {
                    Code = 200,
                    Status = "success",
                    Message = "Category added successfully."
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    Code = 500,
                    Status = "error",
                    Message = $"Error: {ex.Message}"
                };
            }
        }
        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _dbcontext.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            return categories.Select(c => new CategoryDTO
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            });
        }


        public async Task<APIResponse> AdminLoginAsync(AdminLoginDTO model)
        {
            try
            {
                // Check if the admin exists in the database
                var admin = await _dbcontext.AdminLogins
                    .FirstOrDefaultAsync(a => a.Email == model.Email && a.Password == model.Password);

                if (admin == null)
                {
                    return new APIResponse
                    {
                        Code = 401,
                        Status = "error",
                        Message = "Invalid email or password."
                    };
                }

                return new APIResponse
                {
                    Code = 200,
                    Status = "success",
                    Message = "Login successful."
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    Code = 500,
                    Status = "error",
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<LoginResponse> AdminLoginAsync(AdminLoginRequestDto loginDto)
        {
            var admin = await _dbcontext.AdminLogins
                .FirstOrDefaultAsync(a => a.Email == loginDto.Email && a.Password == loginDto.Password);

            if (admin == null)
            {
                return new LoginResponse
                {
                    Code = 401,
                    Status = "Failed",
                    Message = "Invalid email or password",
                    Data = null
                };
            }

            return new LoginResponse
            {
                Code = 200,
                Status = "Success",
                Message = "Login successful",
                Data = new { admin.AdminId, admin.Email }
            };
        }



        public async Task<APIResponse> PurchaseRequestAsync(PurchaseRequestDto model)
        {
            try
            {
                var mailcheck = await _dbcontext.Students.Where(x => x.Email == model.Email).FirstOrDefaultAsync();
                if (mailcheck == null)
                {
                    return new APIResponse { Code = 400, Message = "This email id not registered", Status = "error" };
                }
                if (string.IsNullOrEmpty(model.Email))
                {
                    return new APIResponse { Code = 400, Message = "Email cannot be null", Status = "error" };
                }

                var project = new PurchaseRequest
                {
                    ProjectId = model.ProjectId,
                    Email = model.Email,
                    PurchaseDate = DateTime.UtcNow,
                    IsPremium = model.IsPremium,
                    IsBasic = model.IsBasic,
                    IsStatus = "Pending",
                };

                _dbcontext.PurchaseRequests.Add(project);
                await _dbcontext.SaveChangesAsync();

                //await _emailService.SendEmailAsync(
                //    "New Purchase Request Submitted",
                //    $"A new purchase request was submitted by {model.Email} for ProjectId: {model.ProjectId}."
                //);

                return new APIResponse { Code = 200, Message = "Purches request submitted successfully", Status = "success" };
            }
            catch (Exception ex)
            {
                return new APIResponse { Code = 500, Message = "Error: " + ex.Message, Status = "error" };
            }
        }

        public async Task<IEnumerable<PurchaseRequestWithStudentDto>> GetPurchaseRequestsAsync()
        {
            var requests = await _dbcontext.PurchaseRequests
        .Join(
            _dbcontext.Students,
            pr => pr.Email,
            s => s.Email,
            (pr, s) => new PurchaseRequestWithStudentDto
            {
                PurchaseId = pr.PurchaseId,
                Email = pr.Email,
                ProjectId = pr.ProjectId,
                PurchaseDate = pr.PurchaseDate,
                IsPremium = pr.IsPremium,
                IsStatus = pr.IsStatus,
                StudentId = s.Id,
                Name = s.Name,
                Mobile = s.Mobile,
               
            }
        )
        .OrderByDescending(x => x.PurchaseDate)
        .ToListAsync();
            return requests;


        }

        public async Task<APIResponse> UpdatePurchaseRequestStatusAsync(int purchaseId, string isStatus)
        {
            var entity = await _dbcontext.PurchaseRequests.FirstOrDefaultAsync(x => x.PurchaseId == purchaseId);
            if (entity == null)
            {
                return new APIResponse { Code = 404, Status = "Failed", Message = "Purchase request not found." };
            }

            entity.IsStatus = isStatus;
            await _dbcontext.SaveChangesAsync();

            return new APIResponse { Code = 200, Status = "Success", Message = "Purchase request status updated." };
        }

        public async Task<APIResponse> SoftDeletePurchaseRequestAsync(int purchaseId)
        {
            var entity = await _dbcontext.PurchaseRequests.Where(x => x.PurchaseId == purchaseId).FirstOrDefaultAsync();
            if (entity == null)
            {
                return new APIResponse { Code = 404, Status = "Failed", Message = "Purchase request not found." };
            }

            _dbcontext.PurchaseRequests.Remove(entity); // Correctly remove the entity from the DbContext
            await _dbcontext.SaveChangesAsync(); // Save changes to persist the deletion

            return new APIResponse { Code = 200, Status = "Success", Message = "Purchase request status updated (soft deleted)." };
        }


        // Updated code to fix CS8208 and CS0165 errors
        public async Task<IEnumerable<PurchaseRequestDto>> GetPurchaseRequestsByStudentIdAsync(int studentId)
        {
            // Get the student's email by ID
            var student = await _dbcontext.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null)
                return Enumerable.Empty<PurchaseRequestDto>();

            var requests = await _dbcontext.PurchaseRequests
                .Where(pr => pr.Email == student.Email)
                .Select(pr => new PurchaseRequestDto
                {
                    PurchaseId = pr.PurchaseId,
                    Email = pr.Email,
                    ProjectId = pr.ProjectId,
                    PurchaseDate = pr.PurchaseDate,
                    IsPremium = pr.IsPremium,
                    IsStatus = pr.IsStatus // If IsStatus exists
                })
                .ToListAsync();

            return requests;
        }

       


        public async Task<ProjectRequest?> GetProjectByIdAsync(int id)
        {
            try
            {
                // Fetch the project by ID
                var project = await _dbcontext.ProjectRequests.FirstOrDefaultAsync(p => p.Rpid == id);
                return project;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching project by ID: {ex.Message}");
                return null;
            }
        }

        public async Task<APIResponse> DeleteProjectRequestAsync(int projectRequestId)
        {
            var projectRequest = await _dbcontext.ProjectRequests.FindAsync(projectRequestId);
            if (projectRequest == null)
            {
                return new APIResponse
                {
                    Code = 404,
                    Status = "error",
                    Message = "Project request not found."
                };
            }

            _dbcontext.ProjectRequests.Remove(projectRequest);
            await _dbcontext.SaveChangesAsync();

            return new APIResponse
            {
                Code = 200,
                Status = "success",
                Message = "Project request deleted successfully."
            };
        }
    }
}
