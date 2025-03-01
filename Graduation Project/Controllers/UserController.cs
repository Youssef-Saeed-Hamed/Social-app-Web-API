using Core_Layer.Data_Transfer_Object;
using Core_Layer.Inetrfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service_Layer;
using System.Security.Claims;

namespace Graduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userService;

        public UserController(IUserServices userService)
        {
            _userService = userService;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReturnUsersDto>>> GetAllUsers()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "You Are Not Authoized"
                });
            return Ok(await _userService.GetAllUsers(UserId));
        }

        [HttpGet("{UserId}")]
        public async Task<ActionResult<UserProfileDto>> GetProfile(string UserId)
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id is null)
                return Unauthorized("You are Not Authorized");
            return Ok(await _userService.GetUserProfile(Id , UserId));
        }
        [HttpGet("MyProfile")]
        public async Task<ActionResult<UserProfileDto>> GetMyProfile()
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id is null)
                return Unauthorized("You are Not Authorized");
            return Ok(await _userService.GetMyProfile(Id));
        }
        [HttpGet("UserData")]
        public async Task<ActionResult<UserProfileDto>> GetMyData()
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id is null)
                return Unauthorized("You are Not Authorized");
            return Ok(await _userService.GetUserData(Id));
        }
        [HttpPost]
        public async Task<ActionResult<Response>> UpdateProfie(InputUpdateProfileDto input)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "You Are Not Authoized"
                });
            var response = await _userService.UpdateProfile(input, UserId);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
