using HustleHub.BusinessArea.Interface;
using HustleHub.BusinessArea.Repository;
using HustleHub_API.BusinessLogic.Models;
using HustleHub_API.DBContext.Entities.TableEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            _logger.LogInformation("\n\nAdminProjectController : \n");
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddAdminProject([FromBody] AdminProjectDTO model)
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
    }
}
