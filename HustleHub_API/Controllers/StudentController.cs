using HustleHub.BusinessArea.Interface;
using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub.BusinessArea.Repository;
using HustleHub_API.BusinessLogic.Models.BusinessModels;
using HustleHub_API.DBContext.Entities.TableEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HustleHub_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StudentController> _logger;

        public StudentController(IRepository repository, IConfiguration configuration, ILogger<StudentController> logger)
        {
            _configuration = configuration;
            _repository = repository;
            _logger = logger;
            _logger.LogInformation("\n\n UsersController Logs : \n");
        }

        // GET: api/Student
        [HttpGet]
        [EnableCors("MyCorsPolicy")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var students = await _repository.GetAllStudentsAsync();
            return Ok(students);
        }

        // GET: api/Student/{email}
        [HttpGet("{email}")]
        [EnableCors("MyCorsPolicy")]
        public async Task<ActionResult<Student>> GetStudent(string email)
        {
            var student = await _repository.GetStudentByEmailAsync(email);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        [EnableCors("MyCorsPolicy")]
        // POST: api/Student/Register
        [HttpPost("RegisterStudent")]
        public async Task<IActionResult> RegisterStudent([FromForm] StudentDTO model, IFormFile? profilePicFile)
        {
            var result = await _repository.RegisterStudentAsync(model, profilePicFile);
            return StatusCode(result.Code, result);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> StudentLogin([FromBody] StudentLoginDTO model)
        {
            var result = await _repository.StudentLoginAsync(model);
            return StatusCode(result.Code, result);
        }

    }
}
