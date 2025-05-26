using HustleHub.BusinessArea.Interface;
using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub.BusinessArea.Repository;
using HustleHub_API.BusinessLogic.Models.BusinessModels;
using HustleHub_API.BusinessLogic.Models.DTOs;
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
        public async Task<IActionResult> Submit([FromForm] RequiredProjectDTO model)
        {
            var result = await objRep.SubmitProjectRequestAsync(model);
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


        [Route("PurchesRequest")]
        [HttpPost]
        public async Task<IActionResult> PurchesRequest([FromBody] PurchaseRequestDto model)
        {
            var result = await objRep.PurchaseRequestAsync(model);
            return StatusCode(result.Code, result);
        }

        [HttpGet("GetPurchaseRequests")]
        public async Task<IActionResult> GetPurchaseRequests()
        {
            var projects = await objRep.GetPurchaseRequestsAsync();
            return Ok(projects);
        }


        [HttpPut("UpdatePurchaseRequestStatus")]
        public async Task<IActionResult> UpdatePurchaseRequestStatus([FromQuery] int purchaseId, [FromQuery] string isStatus)
        {
            var result = await objRep.UpdatePurchaseRequestStatusAsync(purchaseId, isStatus);
            return StatusCode(result.Code, result);
        }

        [HttpDelete("DeletePurchaseRequest")]
        public async Task<IActionResult> DeletePurchaseRequest([FromQuery] int purchaseId)
        {
            var result = await objRep.SoftDeletePurchaseRequestAsync(purchaseId);
            return StatusCode(result.Code, result);
        }


    }
}
