using BallastLane.ReminderApp.Application.Dtos;
using BallastLane.ReminderApp.Application.Exceptions;
using BallastLane.ReminderApp.Application.Interfaces;
using BallastLane.ReminderApp.Application.Services;
using BallastLane.ReminderApp.Domain.Entities;
using BallastLane.UserApp.Application.Interfaces;
using NSubstitute;

namespace BallastLane.ReminderApp.Application.UnitTests
{
    public class UserServiceTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly IJwtProvider _jwtProvider;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _passwordHasher = Substitute.For<IPasswordHasher>();
            _mapper = Substitute.For<IMapper>();
            _jwtProvider = Substitute.For<IJwtProvider>();
            _userService = new UserService(_userRepository, _passwordHasher, _mapper, _jwtProvider);
        }

        [Fact]
        public async Task GetByIdAsync_WithEmptyId_ShouldThrowValidationException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.GetByIdAsync(Guid.Empty));

            Assert.Equal("The userId is not valid.", exception.Message);
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepository.GetByIdAsync(userId).Returns(default(User));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _userService.GetByIdAsync(userId));

            Assert.Contains(userId.ToString(), exception.Message);
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserExists_ShouldReturnUserResponseDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                Id = userId,
                Username = "joel",
                Email = "joel@asdf.com",
                Reminders = new List<Reminder>()
            };
            _userRepository.GetByIdAsync(userId).Returns(existingUser);

            var expectedResponse = new UserResponseDto(
                existingUser.Id,
                existingUser.Username,
                existingUser.Email,
                new List<ReminderResponseDto>()
            );

            _mapper.Map<UserResponseDto>(existingUser)
                   .Returns(expectedResponse);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Id, result.Id);
            Assert.Equal(expectedResponse.Username, result.Username);
            Assert.Equal(expectedResponse.Email, result.Email);
        }

        [Fact]
        public async Task AddAsync_WithInvalidData_ShouldThrowValidationExceptionWithAccumulatedErrors()
        {
            // Arrange
            var invalidRequest = new UserRequestDto("", "invalid.email", "123");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.AddAsync(invalidRequest));

            Assert.NotNull(exception.Errors);
            Assert.True(exception.Errors.ContainsKey(nameof(UserRequestDto.Username)));
            Assert.True(exception.Errors.ContainsKey(nameof(UserRequestDto.Email)));
            Assert.True(exception.Errors.ContainsKey(nameof(UserRequestDto.Password)));
        }

        [Fact]
        public async Task AddAsync_WhenEmailAlreadyExists_ShouldThrowValidationException()
        {
            // Arrange
            var request = new UserRequestDto("newuser", "existing@example.com", "securepassword");
            _userRepository.ExistsEmailAsync(request.Email).Returns(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.AddAsync(request));

            Assert.True(exception.Errors.ContainsKey(nameof(UserRequestDto.Email)));
            Assert.Equal("The email is already being used.", exception.Errors[nameof(UserRequestDto.Email)][0]);
        }

        [Fact]
        public async Task AddAsync_WithValidData_ShouldHashPasswordAndSaveUser()
        {
            // Arrange
            var request = new UserRequestDto("cleanCoder", "clean@asdf.com", "password123");
            string mockHash = "hashed-password.xyz";

            _userRepository.ExistsEmailAsync(request.Email).Returns(false);
            _passwordHasher.HashPassword(request.Password).Returns(mockHash);

            // Act
            await _userService.AddAsync(request);

            // Assert
            _passwordHasher.Received(1).HashPassword(request.Password);

            await _userRepository.Received(1).CreateAsync(Arg.Is<User>(u =>
                u.Username == request.Username &&
                u.Email == request.Email &&
                u.Password == mockHash &&
                u.Id != Guid.Empty
            ));
        }

        [Fact]
        public async Task UpdateAsync_WhenChangingEmailToAnAlreadyUsedOne_ShouldAccumulateErrors()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User { Id = userId, Username = "oldName", Email = "old@asdf.com" };

            var updateRequest = new UserRequestDto("newName", "taken@asdf.com", "");

            _userRepository.GetByIdAsync(userId).Returns(existingUser);
            _userRepository.ExistsEmailAsync(updateRequest.Email).Returns(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.UpdateAsync(userId, updateRequest));

            Assert.True(exception.Errors.ContainsKey(nameof(UserRequestDto.Email)));
        }

        [Fact]
        public async Task UpdateAsync_WithValidDataAndPassword_ShouldUpdateFieldsAndHashNewPassword()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User { Id = userId, Username = "user", Email = "user@asdf.com", Password = "old*hash" };
            var updateRequest = new UserRequestDto("new-user", "user@asdf.com", "new*password");
            string newHash = "new/hash*123";

            _userRepository.GetByIdAsync(userId).Returns(existingUser);
            _passwordHasher.HashPassword(updateRequest.Password).Returns(newHash);

            // Act
            await _userService.UpdateAsync(userId, updateRequest);

            // Assert
            Assert.Equal("new-user", existingUser.Username);
            Assert.Equal(newHash, existingUser.Password);
            await _userRepository.Received(1).UpdateAsync(existingUser);
        }
    }
}
