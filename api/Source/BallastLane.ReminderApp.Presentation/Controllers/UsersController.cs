using BallastLane.ReminderApp.Application.Dtos;
using BallastLane.ReminderApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.ReminderApp.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _service;

        public UsersController(IUserService service, ILogger<UsersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
        {
            var reminders = await _service.GetAllAsync();
            return Ok(reminders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetById(Guid id)
        {
            _logger.LogInformation("Fetching user with Id: {UserId}", id);
            var user = await _service.GetByIdAsync(id);
            return Ok(user);
        }

        [HttpGet("search")]
        public async Task<ActionResult<UserResponseDto>> GetByEmail([FromQuery] string email)
        {
            _logger.LogInformation("Searching user by email: {Email}", email);
            var user = await _service.GetByEmailAsync(email);
            return Ok(user);
        }

        [HttpGet("exists")]
        public async Task<ActionResult<bool>> Exists([FromQuery] string email)
        {
            _logger.LogInformation("Verify if the user with Email: {email} exists", email);
            var exists = await _service.ExistsAsync(email);
            return Ok(exists);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserRequestDto userDto)
        {
            _logger.LogInformation("Creating a new user with email: {Email}", userDto.Email);

            var created = await _service.AddAsync(userDto);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UserRequestDto userDto)
        {
            _logger.LogInformation("Updating user with Id: {UserId}", id);
            await _service.UpdateAsync(id, userDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Deleting user with Id: {UserId}", id);
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
