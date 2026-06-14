namespace BallastLane.ReminderApp.Application.Dtos
{
    public record UserRequestDto(string Username, string Email, string Password);

    public record UserResponseDto(Guid Id, string Username, string Email, List<ReminderResponseDto> Reminders);
}
