using Core_Layer.Data_Transfer_Object;
using Core_Layer.Inetrfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service_Layer;
using System.Security.Claims;

namespace Graduation_Project.Controllers
{
    [Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class FollowingController : ControllerBase
    {
        private readonly IFollowingService _followingService;

        public FollowingController(IFollowingService followingService)
        {
            _followingService = followingService;
        }

        [HttpPost]
        public async Task<ActionResult<Response>> Send(InputFollowerDto input)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "ليس لديك صلاحية الوصول"
                });
            var response = await _followingService.SendFollow(input, UserId);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
        [HttpDelete("CancelFollower")]
        public async Task<ActionResult<Response>> CancelFollow(InputFollowerDto input)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "ليس لديك صلاحية الوصول"
                });
            var response = await _followingService.CancelFollow(input, UserId);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
        [HttpDelete("CancelFollow")]
        public async Task<ActionResult<Response>> CancelFollower(InputFollowerDto input)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "ليس لديك صلاحية الوصول"
                });
            var response = await _followingService.CancelFollower(input, UserId);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
        //[HttpDelete("Request")]
        //public async Task<ActionResult<ReturnFollowerMessageDto>> CancelREquest(InputFollowerDto input)
        //{
        //    var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    return Ok(await _followingService.CancelRequest(input, UserId));
        //}
        //[HttpPut]
        //public async Task<ActionResult<ReturnFollowerMessageDto>> Accept(InputFollowerDto input)
        //{
        //    var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    return Ok(await _followingService.AcceptFollow(input, UserId));
        //}
        //[HttpDelete("RefuseRequest")]
        //public async Task<ActionResult<ReturnFollowerMessageDto>> Refuse(InputFollowerDto input)
        //{
        //    var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    return Ok(await _followingService.RefuseFollow(input, UserId));
        //}

        [HttpGet("Followers")]
        public async Task<ActionResult<IEnumerable<UserPostDto>>> Followers()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await _followingService.GetFollowers(UserId!));
        }


        [HttpGet("Followings")]
        public async Task<ActionResult<IEnumerable<UserPostDto>>> Followings()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await _followingService.GetFollowings(UserId!));
        }

        //[HttpGet("SentRequests")]
        //public async Task<ActionResult<IEnumerable<UserPostDto>>> SentRequests()
        //{
        //    var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    return Ok(await _followingService.GetRequstsISend(UserId));
        //}

        //[HttpGet("Requests")]
        //public async Task<ActionResult<IEnumerable<UserPostDto>>> Requests()
        //{
        //    var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    return Ok(await _followingService.GetRequstsSendToMe(UserId));
        //}


    }
}
