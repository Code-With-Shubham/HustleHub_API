using HustleHub_API.BusinessLogic.Models.WhatsApp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HustleHub_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly WhatsAppService _whatsAppService;

        public ServicesController()
        {
            _whatsAppService = new WhatsAppService();
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromQuery] string phone, [FromQuery] string msg)
        {
            var result = await _whatsAppService.SendTextMessageAsync(phone, msg);
            return Ok(result);
        }
    }
}
