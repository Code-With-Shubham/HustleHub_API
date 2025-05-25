using HustleHub.BusinessArea.Interface;
using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub_API.BusinessLogic.Models;
using HustleHub_API.BusinessLogic.Models.APIResponse;
using HustleHub_API.BusinessLogic.Models.BusinessModels;
using HustleHub_API.BusinessLogic.Models.DTOs;
using HustleHub_API.Data;
using HustleHub_API.DBContext.Entities.TableEntities;
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


        public async Task<APIResponse> RegisterStudentAsync(StudentDTO model, IFormFile? profilePicFile)
        {
            APIResponse result = new APIResponse();
            try
            {
                byte[]? profilePicBytes = null;
                if (profilePicFile != null && profilePicFile.Length > 0)
                {
                    if (profilePicFile.Length > 2 * 1024 * 1024)
                        return new APIResponse { Code = 400, Status = "error", Message = "Profile picture size must be less than 2MB." };

                    using var ms = new MemoryStream();
                    await profilePicFile.CopyToAsync(ms);
                    profilePicBytes = ms.ToArray();
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


        //Project Requirement

        public async Task<APIResponse> SubmitProjectRequestAsync(RequiredProjectDTO model, IFormFile? projectDocFile)
        {
            try
            {
                var mailcheck = await _dbcontext.Students.Where(x => x.Email == model.Email || x.Mobile == model.Mobile).FirstOrDefaultAsync();
                if (mailcheck == null)
                {
                    return new APIResponse { Code = 400, Message = "This email id not registered", Status = "error" };
                }
                if (string.IsNullOrEmpty(model.Email))
                {
                    return new APIResponse { Code = 400, Message = "Email cannot be null", Status = "error" };
                }

                byte[]? projectDocBytes = null;
                if (projectDocFile != null && projectDocFile.Length > 0)
                {
                    if (projectDocFile.Length > 2 * 1024 * 1024)
                        return new APIResponse { Code = 400, Message = "Project document size must be less than 2MB.", Status = "error" };

                    using var ms = new MemoryStream();
                    await projectDocFile.CopyToAsync(ms);
                    projectDocBytes = ms.ToArray();
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
            var projects = await _dbcontext.ProjectRequests.OrderByDescending(p => p.UpdateDate).ToListAsync();

            foreach (var project in projects)
            {
                if (project.ProjectDocs != null && project.ProjectDocs.Length > 0)
                {
                    // Optionally convert to Base64 for API response
                    // project.ProjectDocs = project.ProjectDocs;
                }
            }

            return projects;
        }
        public async Task<ProjectRequest?> GetProjectByIdAsync(int id) // Updated return type to allow nullability
        {
            var project = await _dbcontext.ProjectRequests
                                          .FirstOrDefaultAsync(p => p.Rpid == id);

            if (project != null && project.ProjectDocs != null) // Fixed type mismatch by checking for null directly
            {
                var documentPath = Path.Combine(_environment.ContentRootPath, "Uploads", "ProjectDocs", Convert.ToBase64String(project.ProjectDocs));
                if (File.Exists(documentPath))
                {
                    var documentBytes = await File.ReadAllBytesAsync(documentPath);
                    project.ProjectDocs = documentBytes; // No conversion to string, keeping it as byte[]
                }
                else
                {
                    project.ProjectDocs = null;
                }
            }

            return project; // No CS8603 warning since the method now explicitly allows null return
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
                    }).FirstOrDefaultAsync();

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
                        ProjectDocsBase64 = p.ProjectDocs != null ? Convert.ToBase64String(p.ProjectDocs) : null,
                        Mobile = p.Mobile,
                        Budget = p.Budget ?? 0m,
                        ApprovedBy = p.ApprovedBy,
                        ApprovedDate = p.ApprovedDate,
                        UpdateAt = p.UpdateDate ?? DateTime.MinValue,
                        UpdateCount = p.UpdateCount
                    }).ToListAsync();

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
                    RequestDate = DateTime.UtcNow,
                    IsPremium = model.IsPremium,
                    IsStatus = "Pending",
                };

                _dbcontext.PurchaseRequests.Add(project);
                await _dbcontext.SaveChangesAsync();

                await _emailService.SendEmailAsync(
                    "New Purchase Request Submitted",
                    $"A new purchase request was submitted by {model.Email} for ProjectId: {model.ProjectId}."
                );

                return new APIResponse { Code = 200, Message = "Purches request submitted successfully", Status = "success" };
            }
            catch (Exception ex)
            {
                return new APIResponse { Code = 500, Message = "Error: " + ex.Message, Status = "error" };
            }
        }

        public async Task<IEnumerable<PurchaseRequest>> GetPurchaseRequestsAsync()
        {
            var projects = await _dbcontext.PurchaseRequests
                                           .OrderByDescending(p => p.RequestDate)
                                           .ToListAsync();
            return projects;
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
                    PurchaseDate = pr.RequestDate,
                    IsPremium = pr.IsPremium,
                    IsStatus = pr.IsStatus // If IsStatus exists
                })
                .ToListAsync();

            return requests;
        }
    }
}
