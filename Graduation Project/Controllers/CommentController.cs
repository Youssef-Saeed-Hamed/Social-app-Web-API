using Core_Layer;
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
    
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<ActionResult<Response>> Add([FromBody] InputCommentDto commentDto)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "You Are Not Authoized"
                });
            var response = await _commentService.InsertCommentAsync(commentDto, UserId);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
        [HttpPost("{ParentCommentId}")]
        public async Task<ActionResult<Response>> Add([FromBody] InputCommentDto commentDto , string ParentCommentId)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "You Are Not Authoized"
                });
            var response = await _commentService.InsertCommentAsync(commentDto, UserId, ParentCommentId);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            
        }
        [HttpPut("{CommentId}")]
        public async Task<ActionResult<Response>> Update(InputCommentDto commentDto , string CommentId)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "You Are Not Authoized"
                });
            var response = await _commentService.UpdateComment(commentDto, UserId, CommentId);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpDelete("CommentId")]
        public async Task<ActionResult<Response>> Delete(string CommentId)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "You Are Not Authoized"
                });
            var response = await _commentService.DeleteComment(UserId, CommentId);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentsDto>>> GetAll([FromQuery] CommentSpecificationParameter parameters)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return BadRequest("User ID Not Correct");
            return Ok(await _commentService.GetCommentsAsync(parameters , UserId));
        }

    }
}
