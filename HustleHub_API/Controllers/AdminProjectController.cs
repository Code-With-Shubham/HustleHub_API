using HustleHub.BusinessArea.Interface;
using HustleHub.BusinessArea.Repository;
using HustleHub_API.BusinessLogic.Models;
using HustleHub_API.BusinessLogic.Models.BusinessModels;
using HustleHub_API.BusinessLogic.Models.DTOs;
using HustleHub_API.DBContext.Entities.TableEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HustleHub_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminProjectController : ControllerBase
    {
        private readonly IRepository objRep;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminProjectController> _logger;

        public AdminProjectController(IRepository repository, IConfiguration configuration, ILogger<AdminProjectController> logger)
        {
            _configuration = configuration;
            objRep = repository;
            _logger = logger;
            _logger.LogInformation("\n\n AdminProjectController : \n");
        }

        [HttpPost("add-admin-project")]
        public async Task<IActionResult> AddAdminProject([FromForm] AdminProjectDTO model)
        {
            var result = await objRep.AddAdminProjectAsync(model);
            return StatusCode(result.Code, result);
        }


        [HttpGet("All")]
        public async Task<IActionResult> GetAllAdminProjects()
        {
            var projects = await objRep.GetAllAdminProjectsAsync();
            return Ok(projects);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetAdminProjectById(int id)
        {
            var project = await objRep.GetAdminProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound(new { Message = "Admin project not found." });
            }
            return Ok(project);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteAdminProject(int id)
        {
            var result = await objRep.DeleteAdminProjectAsync(id);
            return StatusCode(result.Code, result);
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO model)
        {
            var result = await objRep.AddCategoryAsync(model);
            return StatusCode(result.Code, result);
        }

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await objRep.GetAllCategoriesAsync();
            return Ok(categories);
        }

         [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginRequestDto loginDto)
        {
            // Map AdminLoginRequestDto to AdminLoginDTO
            var adminLoginDto = new AdminLoginDTO
            {
                Email = loginDto.Email,
                Password = loginDto.Password
            };

            var response = await objRep.AdminLoginAsync(adminLoginDto);
            if (response.Code == 200)
                return Ok(response);
            return Unauthorized(response);
        }

    }
}
