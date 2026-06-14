using BallastLane.ReminderApp.Application.Dtos;
using BallastLane.ReminderApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.ReminderApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemindersController : ControllerBase
    {
        private readonly ILogger<RemindersController> _logger;
        private readonly IReminderService _service;

        public RemindersController(IReminderService service, ILogger<RemindersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReminderResponseDto>>> GetAll(
            [FromQuery] bool? isCompleted,
            [FromQuery] bool? isOverdue)
        {
            var reminders = await _service.GetRemindersAsync(isCompleted, isOverdue);
            return Ok(reminders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReminderResponseDto>> GetById(Guid id)
        {
            var reminder = await _service.GetReminderByIdAsync(id);
            return Ok(reminder);
        }

        [HttpPost]
        public async Task<ActionResult<ReminderResponseDto>> Create(ReminderRequestDto reminder)
        {
            _logger.LogInformation("Creating a new reminder with Title: {title}", reminder.Title);
            var created = await _service.CreateReminderAsync(reminder);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ReminderRequestDto reminder)
        {
            _logger.LogInformation("Updating reminder with Id: {id}", id);
            await _service.UpdateReminderAsync(id, reminder);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Deleting reminder with Id: {id}", id);
            await _service.DeleteReminderAsync(id);
            return NoContent();
        }
    }
}
