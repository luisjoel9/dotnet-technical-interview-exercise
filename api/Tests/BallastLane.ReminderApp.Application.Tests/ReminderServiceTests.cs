using BallastLane.ReminderApp.Application.Dtos;
using BallastLane.ReminderApp.Application.Exceptions;
using BallastLane.ReminderApp.Application.Interfaces;
using BallastLane.ReminderApp.Application.Services;
using BallastLane.ReminderApp.Domain.Entities;
using BallastLane.ReminderApp.Domain.Enums;
using NSubstitute;

namespace BallastLane.ReminderApp.Application.UnitTests
{
    public class ReminderServiceTests
    {
        private readonly IReminderRepository _repository;
        private readonly IMapper _mapper;
        private readonly ReminderService _reminderService;

        public ReminderServiceTests()
        {
            _repository = Substitute.For<IReminderRepository>();
            _mapper = Substitute.For<IMapper>();
            _reminderService = new ReminderService(_repository, _mapper);
        }

        [Fact]
        public async Task GetRemindersAsync_ShouldReturnMappedCollection()
        {
            // Arrange
            bool? isCompleted = false;
            bool? isOverdue = true;
            var userId = Guid.NewGuid();
            var reminderId = Guid.NewGuid();
            var targetTime = DateTime.Now.AddDays(1);

            var mockReminders = new List<Reminder> { new Reminder { Id = reminderId, Title = "Test Reminder" } };

            var expectedResponse = new List<ReminderResponseDto>
            {
                new ReminderResponseDto(reminderId, userId, "Test Reminder", "Description", targetTime, StatusEnum.Pending)
            };

            _repository.GetAllAsync(isCompleted, isOverdue).Returns(mockReminders);
            _mapper.Map<IEnumerable<ReminderResponseDto>>(mockReminders).Returns(expectedResponse);

            // Act
            var result = await _reminderService.GetRemindersAsync(isCompleted, isOverdue);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task GetReminderByIdAsync_WhenReminderDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var reminderId = Guid.NewGuid();
            _repository.GetByIdAsync(reminderId).Returns(default(Reminder));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _reminderService.GetReminderByIdAsync(reminderId));

            Assert.Contains(reminderId.ToString(), exception.Message);
        }

        [Fact]
        public async Task GetReminderByIdAsync_WhenReminderExists_ShouldReturnMappedDto()
        {
            // Arrange
            var reminderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var targetTime = DateTime.Now.AddDays(1);

            var mockReminder = new Reminder { Id = reminderId, Title = ".NET Tech interview" };

            var expectedDto = new ReminderResponseDto(reminderId, userId, ".NET Tech interview", "Description", targetTime, StatusEnum.Pending);

            _repository.GetByIdAsync(reminderId).Returns(mockReminder);
            _mapper.Map<ReminderResponseDto>(mockReminder).Returns(expectedDto);

            // Act
            var result = await _reminderService.GetReminderByIdAsync(reminderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reminderId, result.Id);
            Assert.Equal(".NET Tech interview", result.Title);
        }

        [Fact]
        public async Task CreateReminderAsync_WithInvalidData_ShouldThrowValidationExceptionWithAccumulatedErrors()
        {
            // Arrange
            var invalidRequest = new ReminderRequestDto(Guid.Empty, "", "   ", DateTime.Now.AddDays(-1));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _reminderService.CreateReminderAsync(invalidRequest));

            Assert.NotNull(exception.Errors);
            Assert.True(exception.Errors.ContainsKey("UserId"));
            Assert.True(exception.Errors.ContainsKey("Title"));
            Assert.True(exception.Errors.ContainsKey("DueDate"));

            Assert.Equal("The userId is required.", exception.Errors["UserId"][0]);
            Assert.Equal("The reminder title is mandatory.", exception.Errors["Title"][0]);
            Assert.Equal("The due date cannot be in the past.", exception.Errors["DueDate"][0]);
        }

        [Fact]
        public async Task CreateReminderAsync_WithValidData_ShouldSetPendingStatusAndReturnCreatedDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var targetTime = DateTime.Now.AddHours(3);
            var expectedId = Guid.NewGuid();

            var requestDto = new ReminderRequestDto(userId, "Learn English", "Learn English this night", targetTime);

            var mappedReminder = new Reminder
            {
                UserId = requestDto.UserId,
                Title = requestDto.Title,
                TargetDateTime = requestDto.TargetDateTime
            };

            var finalResponseDto = new ReminderResponseDto(expectedId, userId, requestDto.Title, "Description", targetTime, StatusEnum.Pending);

            _mapper.Map<Reminder>(requestDto).Returns(mappedReminder);
            _repository.CreateAsync(Arg.Is<Reminder>(r => r.Status == StatusEnum.Pending)).Returns(expectedId);
            _mapper.Map<ReminderResponseDto>(mappedReminder).Returns(finalResponseDto);

            // Act
            var result = await _reminderService.CreateReminderAsync(requestDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedId, result.Id);
            Assert.Equal(StatusEnum.Pending, result.Status);
            Assert.Equal(StatusEnum.Pending, mappedReminder.Status);
        }

        [Fact]
        public async Task UpdateReminderAsync_WhenReminderDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var reminderId = Guid.NewGuid();
            var requestDto = new ReminderRequestDto(Guid.NewGuid(), "Reminder 1", "Reminder 1 description", DateTime.Now.AddHours(1));
            _repository.GetByIdAsync(reminderId).Returns(default(Reminder));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _reminderService.UpdateReminderAsync(reminderId, requestDto));

            Assert.Contains(reminderId.ToString(), exception.Message);
        }

        [Fact]
        public async Task UpdateReminderAsync_WithInvalidData_ShouldThrowValidationException()
        {
            // Arrange
            var reminderId = Guid.NewGuid();
            var existingReminder = new Reminder { Id = reminderId };
            var invalidRequest = new ReminderRequestDto(Guid.Empty, "", "", DateTime.MinValue);

            _repository.GetByIdAsync(reminderId).Returns(existingReminder);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _reminderService.UpdateReminderAsync(reminderId, invalidRequest));

            Assert.True(exception.Errors.ContainsKey("UserId"));
            Assert.True(exception.Errors.ContainsKey("Title"));
        }

        [Fact]
        public async Task UpdateReminderAsync_WithValidData_ShouldCallRepositoryUpdate()
        {
            // Arrange
            var reminderId = Guid.NewGuid();
            var existingReminder = new Reminder { Id = reminderId, Title = "Original" };
            var requestDto = new ReminderRequestDto(Guid.NewGuid(), "Title modified", "Description modified", DateTime.UtcNow.AddMinutes(50));
            var mappedReminderToUpdate = new Reminder { Title = "Modified" };

            _repository.GetByIdAsync(reminderId).Returns(existingReminder);
            _mapper.Map<Reminder>(requestDto).Returns(mappedReminderToUpdate);

            // Act
            await _reminderService.UpdateReminderAsync(reminderId, requestDto);

            // Assert
            Assert.Equal(reminderId, mappedReminderToUpdate.Id);
            await _repository.Received(1).UpdateAsync(mappedReminderToUpdate);
        }

        [Fact]
        public async Task DeleteReminderAsync_WhenReminderDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var reminderId = Guid.NewGuid();
            _repository.GetByIdAsync(reminderId).Returns(default(Reminder));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _reminderService.DeleteReminderAsync(reminderId));

            Assert.Contains(reminderId.ToString(), exception.Message);
        }

        [Fact]
        public async Task DeleteReminderAsync_WhenReminderExists_ShouldCallRepositoryDelete()
        {
            // Arrange
            var reminderId = Guid.NewGuid();
            var existingReminder = new Reminder { Id = reminderId };
            _repository.GetByIdAsync(reminderId).Returns(existingReminder);

            // Act
            await _reminderService.DeleteReminderAsync(reminderId);

            // Assert
            await _repository.Received(1).DeleteAsync(reminderId);
        }
    }
}
