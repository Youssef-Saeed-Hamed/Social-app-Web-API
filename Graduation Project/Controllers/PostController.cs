using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities;
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
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }
        [HttpPost]
        public async Task<ActionResult<PostDto>> AddPost(InputPostDto postDto)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "You Are Not Authoized"
                });
            var response = await _postService.AddPost(postDto, UserId);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetAllPosts([FromQuery]PostSpecificationParameters specs)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return BadRequest("User ID Not Correct");
            return Ok(await _postService.GetAllPosts(specs , UserId));
        }
        [HttpGet("Followings")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetFollowingPosts()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return BadRequest("User ID Not Correct");
            return Ok(await _postService.GetAllPostsForMyFollowings(UserId));
        }
        [HttpPost("{Id}")]
        public async Task<ActionResult<PostDto>> UpdatePost(UpdatePostDto dto ,string Id)
        {

            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "You Are Not Authoized"
                });
            var response = await _postService.UpdatePost(dto, Id, UserId);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeletePost(DeletePostDto dto)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "You Are Not Authoized"
                });
            var response = await _postService.DeletePost(dto, UserId);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);

        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<PostDto>> GetPost(string Id)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok(await _postService.GetPost(Id , UserId!));

        }
    }
}
