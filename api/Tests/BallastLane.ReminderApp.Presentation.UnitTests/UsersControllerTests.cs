using BallastLane.ReminderApp.Application.Dtos;
using BallastLane.ReminderApp.Application.Services;
using BallastLane.ReminderApp.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BallastLane.ReminderApp.Presentation.UnitTests
{
    public class UsersControllerTests
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userService = Substitute.For<IUserService>();
            _logger = Substitute.For<ILogger<UsersController>>();

            _controller = new UsersController(
                _userService,
                _logger);
        }
        [Fact]
        public async Task GetAll_Should_Return_Ok_With_User()
        {
            // Arrange
            var users = new List<UserResponseDto>
            {
                new(Guid.NewGuid(), "joel", "joel@asdf.com", []),
                new(Guid.NewGuid(), "pepe", "pepe@asdf.com", [])
            };

            _userService.GetAllAsync()
                .Returns(users);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsAssignableFrom<IEnumerable<UserResponseDto>>(okResult.Value);

            Assert.Equal(2, value.Count());
        }

        [Fact]
        public async Task GetById_Should_Return_Ok_When_User_Exist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var user = new UserResponseDto(
                userId,
                "joel",
                "joel@asdf.com",
                []);

            _userService.GetByIdAsync(userId)
                .Returns(user);

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var returnedUser =
                Assert.IsType<UserResponseDto>(okResult.Value);

            Assert.Equal(userId, returnedUser.Id);
        }

        [Fact]
        public async Task GetByEmail_Should_Return_Ok_When_User_Exists()
        {
            // Arrange
            var email = "joel@asdf.com";

            var user = new UserResponseDto(
                Guid.NewGuid(),
                "joel",
                email,
                []);

            _userService.GetByEmailAsync(email)
                .Returns(user);

            // Act
            var result = await _controller.GetByEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var returnedUser =
                Assert.IsType<UserResponseDto>(okResult.Value);

            Assert.Equal(email, returnedUser.Email);
        }

        [Fact]
        public async Task Exists_Should_Return_True_When_UserExists()
        {
            // Arrange
            const string email = "joel@asdf.com";

            _userService.ExistsAsync(email)
                .Returns(true);

            // Act
            var result = await _controller.Exists(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            Assert.True((bool)okResult.Value!);
        }

        [Fact]
        public async Task Create_Should_Return_CreatedAtAction()
        {
            // Arrange
            var createdUser = new UserResponseDto(
                Guid.NewGuid(),
                "joel",
                "joel@asdf.com",
                []);

            var request = new UserRequestDto(
                "joel",
                "joel@asdf.com",
                "123456");

            _userService.AddAsync(request)
                .Returns(createdUser);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult =
                Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal(nameof(UsersController.GetById),
                createdResult.ActionName);

            var value =
                Assert.IsType<UserResponseDto>(createdResult.Value);

            Assert.Equal(createdUser.Id, value.Id);
        }

        [Fact]
        public async Task Update_Should_Return_NoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new UserRequestDto(
                "joel",
                "joel@asdf.com",
                "123456");

            // Act
            var result = await _controller.Update(userId, request);

            // Assert
            Assert.IsType<NoContentResult>(result);

            await _userService
                .Received(1)
                .UpdateAsync(userId, request);
        }

        [Fact]
        public async Task Delete_Should_Return_NoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);

            await _userService
                .Received(1)
                .DeleteAsync(userId);
        }
    }
}