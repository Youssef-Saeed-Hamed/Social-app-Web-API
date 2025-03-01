using Core_Layer.Data_Transfer_Object;
using Core_Layer.Inetrfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_Project.Controllers
{
    [Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> Notifications()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await _notificationService.GetAllNotifications(UserId));
        }
    }
}
