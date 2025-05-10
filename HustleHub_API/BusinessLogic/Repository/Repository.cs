using HustleHub.BusinessArea.Interface;
using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub_API.BusinessLogic.Models;
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


        public async Task<APIResponse> RegisterStudentAsync(StudentDTO model, IFormFile? profilePicFile)
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

        public async Task<APIResponse> SubmitProjectRequestAsync(RequiredProjectDTO model, IFormFile? projectDocFile)
        {
            try
            {
                var mailcheck = await _dbcontext.Students.Where(x => x.Email == model.Email || x.Mobile == model.Mobile).FirstOrDefaultAsync();
                if(mailcheck == null)
                {
                    return new APIResponse { Code = 400, Message = "This email id not registered", Status = "error" };
                }
                if (string.IsNullOrEmpty(model.Email))
                {
                    return new APIResponse { Code = 400, Message = "Email cannot be null", Status = "error" };
                }

                if (projectDocFile != null && projectDocFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Directory.GetCurrentDirectory(), "uploads", "ProjectDocs");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(projectDocFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await projectDocFile.CopyToAsync(stream);

                    model.ProjectDocsFile = Path.Combine("uploads", "ProjectDocs", uniqueFileName);
                }

                var project = new ProjectRequest
                {
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
                    UpdateDate = DateTime.UtcNow
                };

                _dbcontext.ProjectRequests.Add(project);
                await _dbcontext.SaveChangesAsync();

                return new APIResponse { Code = 200, Message = "Project request submitted successfully", Status = "success" };
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
                    var documentPath = Path.Combine(_environment.ContentRootPath, "Uploads", "ProjectDocs", project.ProjectDocs);
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
        public async Task<ProjectRequest?> GetProjectByIdAsync(int id) // Updated return type to allow nullability
        {
            var project = await _dbcontext.ProjectRequests
                                          .FirstOrDefaultAsync(p => p.Rpid == id);

            if (project != null && !string.IsNullOrEmpty(project.ProjectDocs))
            {
                var documentPath = Path.Combine(_environment.ContentRootPath, "Uploads", "ProjectDocs", project.ProjectDocs);
                if (File.Exists(documentPath))
                {
                    var documentBytes = await File.ReadAllBytesAsync(documentPath);
                    project.ProjectDocs = Convert.ToBase64String(documentBytes);
                }

                else
                {
                    project.ProjectDocs = null;
                }
            }

            return project; // No CS8603 warning since the method now explicitly allows null return
        }





        //Admin Project
        public async Task<APIResponse> AddAdminProjectAsync(AdminProjectDTO model, IFormFile? ProjectIconImage)
        {
            var strategy = _dbcontext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _dbcontext.Database.BeginTransactionAsync();

                try
                {
                    string? ProjectIcon = null;

                    // Upload Project Icon before starting DB operations
                    if (ProjectIconImage != null && ProjectIconImage.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads", "ProjectIcons");

                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        ProjectIcon = model.ProjectName + Guid.NewGuid().ToString() + Path.GetExtension(ProjectIconImage.FileName);
                        var filePath = Path.Combine(uploadsFolder, ProjectIcon);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ProjectIconImage.CopyToAsync(stream);
                        }
                    }

                    model.CreatedAt = DateTime.UtcNow;
                    model.UpdatedAt = null;

                    var obj = new AdminProject
                    {
                        YoutubeLink = model.YoutubeLink,
                        ProjectName = model.ProjectName,
                        LearningOutcomes = model.LearningOutcomes,
                        Description1 = model.Description1,
                        LongDescription = model.LongDescription,
                        Description2 = model.Description2,
                        Image = ProjectIcon,
                        Category = model.Category,
                        BasePrice = model.BasePrice,
                        PremiumPrice = model.PremiumPrice,
                        CreatedAt = model.CreatedAt,
                        UpdatedAt = model.UpdatedAt,
                        DisplayStatus = true
                    };

                    _dbcontext.AdminProjects.Add(obj);
                    await _dbcontext.SaveChangesAsync();

                    if (model.Skills != null && model.Skills.Any())
                    {
                        var projectSkills = model.Skills.Select(skill => new ProjectSkill
                        {
                            ProjectId = obj.ProjectId,
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
                if (!string.IsNullOrEmpty(project.Image))
                {
                    var filePath = Path.Combine(_environment.ContentRootPath, "Uploads", "projectIcons", project.Image);
                    if (File.Exists(filePath))
                    {
                        var bytes = await File.ReadAllBytesAsync(filePath);
                        imageBase64 = Convert.ToBase64String(bytes);
                    }
                }

                // Get skills
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
                    Category = project.Category,
                    LearningOutcomes = project.LearningOutcomes,
                    BasePrice = project.BasePrice,
                    PremiumPrice = project.PremiumPrice,
                    CreatedAt = project.CreatedAt,
                    Image = imageBase64,
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
            if (!string.IsNullOrEmpty(project.Image))
            {
                var filePath = Path.Combine(_environment.ContentRootPath, "Uploads", "projectIcons", project.Image);
                if (File.Exists(filePath))
                {
                    var bytes = await File.ReadAllBytesAsync(filePath);
                    imageBase64 = Convert.ToBase64String(bytes);
                }
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
                Category = project.Category,
                LearningOutcomes = project.LearningOutcomes,
                BasePrice = project.BasePrice,
                PremiumPrice = project.PremiumPrice,
                CreatedAt = project.CreatedAt,
                Image = imageBase64,
                Skills = skills
            };
        }
        public async Task<APIResponse> DeleteAdminProjectAsync(int projectId)
        {
            try
            {
                // Find the AdminProject by ID
                var project = await _dbcontext.AdminProjects
                    .Include(p => p.ProjectSkills) // Include related skills
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

                // Delete related skills
                if (project.ProjectSkills != null && project.ProjectSkills.Any())
                {
                    _dbcontext.ProjectSkills.RemoveRange(project.ProjectSkills);
                }

                // Delete the project icon file if it exists
                if (!string.IsNullOrEmpty(project.Image))
                {
                    var filePath = Path.Combine(_environment.ContentRootPath, "Uploads", "ProjectIcons", project.Image);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                // Delete the AdminProject
                _dbcontext.AdminProjects.Remove(project);
                await _dbcontext.SaveChangesAsync();

                return new APIResponse
                {
                    Code = 200,
                    Status = "success",
                    Message = "Admin project deleted successfully."
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




    }
}
