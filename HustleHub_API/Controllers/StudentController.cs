using HustleHub.BusinessArea.Interface;
using HustleHub.BusinessArea.Models.APIResponse;
using HustleHub.BusinessArea.Repository;
using HustleHub_API.BusinessLogic.Models.BusinessModels;
using HustleHub_API.DBContext.Entities.TableEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HustleHub_API.Controllers
{
    [Authorize]
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
            _logger.LogInformation("\n\n StudentController Logs : \n");
        }

        // GET: api/Student
        [HttpGet]
        [EnableCors("MyCorsPolicy")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var students = await _repository.GetAllStudentsAsync();
            return Ok(students);
        }

        // GET: api/Student/{email}
        [HttpGet("{email}")]
        [EnableCors("MyCorsPolicy")]
        [AllowAnonymous]
        public async Task<ActionResult<Student>> GetStudent(string email)
        {
            var student = await _repository.GetStudentByEmailAsync(email);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // POST: api/Student/Register
        [HttpPost("RegisterStudent")]
        [EnableCors("MyCorsPolicy")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterStudent([FromForm] StudentDTO model)
        {
            var result = await _repository.RegisterStudentAsync(model);
            return StatusCode(result.Code, result);
        }

        // POST: api/Student/Login
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> StudentLogin([FromBody] StudentLoginDTO model)
        {
            var result = await _repository.StudentLoginAsync(model);

            if (result.Code != 200 || result.Data == null)
            {
                return StatusCode(result.Code, result);
            }

            var student = (Student)result.Data;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, student.Name),
                    new Claim(ClaimTypes.Email, student.Email),
                    new Claim("StudentId", student.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Token = jwtToken,
                Student = student
            });
        }

        // GET: api/Student/GetById/{id}
        [HttpGet("GetById/{id}")]
        [EnableCors("MyCorsPolicy")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _repository.GetStudentByIdAsync(id);

            if (student == null)
            {
                return NotFound(new { Message = "Student not found." });
            }

            return Ok(student);
        }

        // DELETE: api/Student/Delete/{id}
        [HttpDelete("Delete/{id}")]
        [EnableCors("MyCorsPolicy")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _repository.DeleteStudentAsync(id);
            return StatusCode(result.Code, result);
        }

        // GET: api/Student/ProjectRequestbyID/{id}
        [HttpGet("ProjectRequestbyID/{id}")]
        [EnableCors("MyCorsPolicy")]
        public async Task<IActionResult> ProjectRequestByID(int id)
        {
            var result = await _repository.ProjectRequestByIDAsync(id);
            return StatusCode(result.Code, result);
        }

        [HttpGet("purchase-requests/by-student/{studentId}")]
        public async Task<IActionResult> GetPurchaseRequestsByStudentId(int studentId)
        {
            var result = await _repository.GetPurchaseRequestsByStudentIdAsync(studentId);
            return Ok(result);
        }
    }
}
