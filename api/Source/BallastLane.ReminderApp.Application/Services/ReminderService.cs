using BallastLane.ReminderApp.Application.Dtos;
using BallastLane.ReminderApp.Application.Exceptions;
using BallastLane.ReminderApp.Application.Interfaces;
using BallastLane.ReminderApp.Domain.Entities;
using BallastLane.ReminderApp.Domain.Enums;

namespace BallastLane.ReminderApp.Application.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRepository _repository;
        private readonly IMapper _mapper;

        public ReminderService(IReminderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReminderResponseDto>> GetRemindersAsync(Guid userId, bool? isCompleted, bool? isOverdue)
        {
            if (userId == Guid.Empty)
            {
                throw new ValidationException("The userId is not valid.");
            }

            var result = await _repository.GetAllAsync(userId, isCompleted, isOverdue);
            return _mapper.Map<IEnumerable<ReminderResponseDto>>(result);
        }

        public async Task<ReminderResponseDto> GetReminderByIdAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null)
                throw new NotFoundException($"Reminder with Id '{id}' was not found.");

            return _mapper.Map<ReminderResponseDto>(result);
        }

        public async Task<ReminderResponseDto> CreateReminderAsync(ReminderRequestDto reminder)
        {
            var validationErrors = new Dictionary<string, string[]>();

            if (reminder.UserId == Guid.Empty)
            {
                validationErrors.Add("UserId", ["The userId is required."]);
            }

            if (string.IsNullOrWhiteSpace(reminder.Title))
            {
                validationErrors.Add("Title", ["The reminder title is mandatory."]);
            }

            if (reminder.TargetDateTime < DateTime.Now)
            {
                validationErrors.Add("DueDate", ["The due date cannot be in the past."]);
            }

            if (validationErrors.Count > 0)
            {
                throw new ValidationException(validationErrors);
            }

            var reminderRequest = _mapper.Map<Reminder>(reminder);
            reminderRequest.Status = StatusEnum.Pending;
            var id = await _repository.CreateAsync(reminderRequest);

            reminderRequest.Id = id;
            return _mapper.Map<ReminderResponseDto>(reminderRequest);
        }

        public async Task UpdateReminderAsync(Guid id, ReminderRequestDto reminder)
        {
            var existingReminder = await _repository.GetByIdAsync(id);
            if (existingReminder == null)
                throw new NotFoundException($"Unable to update. The reminder with ID '{id}' was not found.");

            var validationErrors = new Dictionary<string, string[]>();

            if (reminder.UserId == Guid.Empty)
                validationErrors.Add("UserId", ["The userId is required."]);

            if (string.IsNullOrWhiteSpace(reminder.Title))
                validationErrors.Add("Title", ["The updated reminder title cannot be empty."]);

            if (validationErrors.Count > 0)
                throw new ValidationException(validationErrors);

            var reminderToUpdate = _mapper.Map<Reminder>(reminder);
            reminderToUpdate.Id = id;

            await _repository.UpdateAsync(reminderToUpdate);
        }

        public async Task DeleteReminderAsync(Guid id)
        {
            var existingReminder = await _repository.GetByIdAsync(id);
            if (existingReminder == null)
                throw new NotFoundException($"Unable to delete, the reminder with ID {id} does not exist.");

            await _repository.DeleteAsync(id);
        }
    }
}
