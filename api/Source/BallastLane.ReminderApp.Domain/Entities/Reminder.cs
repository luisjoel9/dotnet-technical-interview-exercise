using BallastLane.ReminderApp.Domain.Enums;

namespace BallastLane.ReminderApp.Domain.Entities
{
    public class Reminder
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TargetDateTime { get; set; }
        public StatusEnum Status { get; set; }
    }
}
