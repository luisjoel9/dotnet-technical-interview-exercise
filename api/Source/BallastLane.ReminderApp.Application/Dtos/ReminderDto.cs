using BallastLane.ReminderApp.Domain.Enums;
namespace BallastLane.ReminderApp.Application.Dtos
{
    public record ReminderRequestDto(Guid UserId, string Title, string Description, DateTime TargetDateTime);

    public record ReminderResponseDto(Guid Id, Guid UserId, string Title, string Description, DateTime TargetDateTime, StatusEnum Status);
}
