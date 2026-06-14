using System.Security.Claims;
using BallastLane.ReminderApp.Application.Dtos;
using BallastLane.ReminderApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.ReminderApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RemindersController : ApiControllerBase
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
            _logger.LogInformation("Fetching reminders for authenticated User: {userId}", CurrentUserId);
            var reminders = await _service.GetRemindersAsync(CurrentUserId, isCompleted, isOverdue);
            return Ok(reminders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReminderResponseDto>> GetById(Guid id)
        {
            var reminder = await _service.GetReminderByIdAsync(id);
            if (reminder.UserId != CurrentUserId)
            {
                return Forbid();
            }
            return Ok(reminder);
        }

        [HttpPost]
        public async Task<ActionResult<ReminderResponseDto>> Create(ReminderRequestDto reminder)
        {
            _logger.LogInformation("Creating a new reminder with Title: {title}", reminder.Title);

            var reminderWithUser = reminder with
            {
                UserId = CurrentUserId
            };

            var created = await _service.CreateReminderAsync(reminderWithUser);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ReminderRequestDto reminder)
        {
            var currentReminder = await _service.GetReminderByIdAsync(id);
            if (currentReminder.UserId != CurrentUserId) return Forbid();

            _logger.LogInformation("Updating reminder with Id: {id}", id);
            await _service.UpdateReminderAsync(id, reminder);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var currentReminder = await _service.GetReminderByIdAsync(id);
            if (currentReminder.UserId != CurrentUserId) return Forbid();
            _logger.LogInformation("Deleting reminder with Id: {id}", id);
            await _service.DeleteReminderAsync(id);
            return NoContent();
        }
    }
}
