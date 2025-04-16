using HustleHub.BusinessArea.Interface;
using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub.BusinessArea.Models.BusinessModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HustleHub_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepository objRep;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IRepository repository, IConfiguration configuration, ILogger<UsersController> logger)
        {
            _configuration = configuration;
            objRep = repository;
            _logger = logger;
            _logger.LogInformation("\n\nUsersController Logs : \n");
        }

        [Route("UserRegistration")]
        [HttpPost]
        public async Task<APIResponse> RegisterUser([FromBody] Users model)
        {
            APIResponse result = new APIResponse();

            try
            {

                result = await objRep.RegisterUserAsync(model);
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
