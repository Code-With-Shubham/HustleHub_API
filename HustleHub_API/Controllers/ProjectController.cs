using HustleHub.BusinessArea.Interface;
using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub_API.BusinessLogic.Models.BusinessModels;
using HustleHub_API.DBContext.Entities.TableEntities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HustleHub_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(IRepository repository, IConfiguration configuration, ILogger<ProjectController> logger)
        {
            _configuration = configuration;
            _repository = repository;
            _logger = logger;
            _logger.LogInformation("\n\nRPController Logs : \n");
        }


        // GET: api/ProjectRequest/GetAllProject
        [HttpGet("GetAllProject")]
        [EnableCors("MyCorsPolicy")]
        public async Task<ActionResult<IEnumerable<ProjectRequest>>> GetAllProjectRequests()
        {
            var projectRequests = await _repository.GetProjectRequestsAsync();
            return Ok(projectRequests);
        }

        // GET: api/ProjectRequest/GetProjectRequestById/5
        [HttpGet("GetProjectRequestById/{id}")]
        [EnableCors("MyCorsPolicy")]
        public async Task<ActionResult<ProjectRequest>> GetProjectRequestById(int id)
        {
            var projectRequest = await _repository.GetProjectRequestByIdAsync(id);
            if (projectRequest == null)
            {
                return NotFound();
            }

            return Ok(projectRequest);
        }

        // POST: api/ProjectRequest/ProjectRequest
        [HttpPost("ProjectRequest")]
        public async Task<ActionResult<APIResponse>> CreateProjectRequest([FromForm] ProjectRequestVM model, IFormFile projectDocsFile)
        {
            var response = await _repository.CreateProjectRequestAsync(model, projectDocsFile);
            if (response.Code == 200)
            {
                return Ok(response);
            }

            return StatusCode(response.Code, response);
        }
    }
}
