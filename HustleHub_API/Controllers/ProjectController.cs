using HustleHub.BusinessArea.Interface;
using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub.BusinessArea.Repository;
using HustleHub_API.BusinessLogic.Models.BusinessModels;
using HustleHub_API.DBContext.Entities.TableEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HustleHub_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IRepository objRep;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProjectController> _logger;


        public ProjectController(IRepository repository, IConfiguration configuration, ILogger<ProjectController> logger)
        {
            _configuration = configuration;
            objRep = repository;
            _logger = logger;
            _logger.LogInformation("\n\nRPController Logs : \n");
        }


        [Route("RegisterProject")]
        [HttpPost]
        public async Task<IActionResult> Submit([FromForm] RequiredProjectDTO model, IFormFile? projectDocsFile)
        {
            var result = await objRep.SubmitProjectRequestAsync(model, projectDocsFile);
            return StatusCode(result.Code, result);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await objRep.GetAllProjectsAsync();
            return Ok(projects);
        }

        [HttpGet("GetProject")]
        public async Task<IActionResult> GetProjects([FromHeader] int id)
        {
            var projects = await objRep.GetProjectByIdAsync(id);
            return Ok(projects);
        }


    }
}
