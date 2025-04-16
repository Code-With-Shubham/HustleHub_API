using HustleHub.BusinessArea.Interface;
using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub.BusinessArea.Models.BusinessModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HustleHub_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RPController : ControllerBase
    {
        private readonly IRepository objRep;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RPController> _logger;

        public RPController(IRepository repository, IConfiguration configuration, ILogger<RPController> logger)
        {
            _configuration = configuration;
            objRep = repository;
            _logger = logger;
            _logger.LogInformation("\n\nRPController Logs : \n");
        }

        [Route("RequiredProject")]
        [HttpPost]
        public async Task<APIResponse> RequiredProject([FromBody] ProjectReq model)
        {
            APIResponse result = new APIResponse();

            try
            {

                result = await objRep.RequiredProjectAsync(model);
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
    }
}
