using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.ReminderApp.Presentation.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected Guid CurrentUserId
        {
            get
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var parsedGuid))
                {
                    throw new UnauthorizedAccessException("Invalid user identity context inside Token.");
                }

                return parsedGuid;
            }
        }
    }
}
