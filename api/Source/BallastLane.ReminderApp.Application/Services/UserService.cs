using BallastLane.ReminderApp.Application.Dtos;
using BallastLane.ReminderApp.Application.Exceptions;
using BallastLane.ReminderApp.Application.Interfaces;
using BallastLane.ReminderApp.Domain.Entities;
using BallastLane.UserApp.Application.Interfaces;

namespace BallastLane.ReminderApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly IJwtProvider _jwtProvider;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IMapper mapper, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _jwtProvider = jwtProvider;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var result = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserResponseDto>>(result);
        }

        public async Task<UserResponseDto> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ValidationException("The userId is not valid.");
            }

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException($"The user with Id '{id}' was not found.");
            }
            var result = _mapper.Map<UserResponseDto>(user);
            return result;
        }

        public async Task<UserResponseDto> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                throw new ValidationException("The email format is not valid.");
            }

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                throw new NotFoundException($"The user with email '{email}' was not found.");
            }

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<bool> ExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return await _userRepository.ExistsEmailAsync(email);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errors.Add(nameof(request.Email), new[] { "Email field cannot be empty." });
            }
            else if (!request.Email.Contains("@"))
            {
                errors.Add(nameof(request.Email), new[] { "The email format is not valid." });
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errors.Add(nameof(request.Password), new[] { "Password field cannot be empty." });
            }

            if (errors.Any())
            {
                throw new ValidationException(errors);
            }

            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.Password))
            {
                errors.Add("Credentials", new[] { "User not found or invalid credentials." });
                throw new ValidationException(errors);
            }

            var token = _jwtProvider.GenerateToken(user);

            var response = _mapper.Map<AuthResponseDto>(user) with { Token = token };

            return response;
        }

        public async Task<UserResponseDto> AddAsync(UserRequestDto userDto)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(userDto.Username))
                errors.Add(nameof(userDto.Username), new[] { "The user name is required." });

            if (string.IsNullOrWhiteSpace(userDto.Email) || !userDto.Email.Contains("@"))
                errors.Add(nameof(userDto.Email), new[] { "The email format is not valid." });

            if (string.IsNullOrWhiteSpace(userDto.Password) || userDto.Password.Length < 6)
                errors.Add(nameof(userDto.Password), new[] { "The password should have at least 6 characters." });

            if (!string.IsNullOrWhiteSpace(userDto.Email) && await _userRepository.ExistsEmailAsync(userDto.Email))
            {
                errors.Add(nameof(userDto.Email), new[] { "The email is already being used." });
            }

            if (errors.Count > 0)
            {
                throw new ValidationException(errors);
            }

            string passwordHash = _passwordHasher.HashPassword(userDto.Password);

            var newUser = _mapper.Map<User>(userDto);
            newUser.Password = passwordHash;
            var id = await _userRepository.CreateAsync(newUser);

            newUser.Id = id;
            return _mapper.Map<UserResponseDto>(newUser);
        }

        public async Task UpdateAsync(Guid id, UserRequestDto userDto)
        {
            if (id == Guid.Empty)
            {
                throw new ValidationException("The user id is required.");
            }

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new NotFoundException($"Unable to update. The user with Id '{id}' was not found.");
            }

            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(userDto.Username))
                errors.Add(nameof(userDto.Username), new[] { "The user name is required." });

            if (string.IsNullOrWhiteSpace(userDto.Email) || !userDto.Email.Contains("@"))
                errors.Add(nameof(userDto.Email), new[] { "The email format is not valid." });

            if (!string.IsNullOrWhiteSpace(userDto.Email) &&
                userDto.Email.ToLower() != existingUser.Email.ToLower() &&
                await _userRepository.ExistsEmailAsync(userDto.Email))
            {
                errors.Add(nameof(userDto.Email), new[] { "The new email address is already being used." });
            }

            bool updatePassword = !string.IsNullOrEmpty(userDto.Password);
            if (updatePassword)
            {
                if (userDto.Password.Length < 6)
                    errors.Add(nameof(userDto.Password), new[] { "The password should have more at least 6 characters." });
            }

            if (errors.Count > 0)
            {
                throw new ValidationException(errors);
            }

            existingUser.Username = userDto.Username;
            existingUser.Email = userDto.Email;

            if (updatePassword)
            {
                existingUser.Password = _passwordHasher.HashPassword(userDto.Password);
            }

            await _userRepository.UpdateAsync(existingUser);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                throw new NotFoundException($"Unable to delete, the user with ID {id} does not exist.");

            await _userRepository.DeleteAsync(id);
        }
    }
}
