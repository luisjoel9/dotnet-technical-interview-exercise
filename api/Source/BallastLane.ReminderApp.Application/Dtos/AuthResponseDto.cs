namespace BallastLane.ReminderApp.Application.Dtos
{
    public record AuthResponseDto(Guid Id, string Username, string Email, string? Token);
}
