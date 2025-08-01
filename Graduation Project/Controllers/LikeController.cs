using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities;
using Core_Layer.Entities.Identity;
using Core_Layer.Inetrfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_Project.Controllers
{
    [Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService _likeService;
        public LikeController(ILikeService likeService, UserManager<AppUser> userManager)
        {
            _likeService = likeService;
        }
        [HttpPost("LikePost")]
        public async Task<IActionResult> LikePost( string PostId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await _likeService.AddLike(PostId, null, userId!));
        }
        [HttpPost("LikeComment")]
        public async Task<IActionResult> LikeComment( string CommentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await _likeService.AddLike(null,CommentId , userId!));
        }
        [HttpDelete("CancelPostLike")]
        public async Task<IActionResult> CancelPostLike( string PostId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await _likeService.CancelLike(PostId, null, userId!));
        }
        [HttpDelete("CancelCommentLike")]
        public async Task<IActionResult> CancelCommentLike( string CommentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await _likeService.CancelLike(null, CommentId, userId!));
        }
       
        [HttpGet("UsersLikePost")]
        public async Task<IActionResult> GetUserLikePost(string PostId)
        {
            return Ok(await _likeService.GetUserLikePostsOrComments(PostId, null));
        }
        [HttpGet("UsersLikeComment")]
        public async Task<IActionResult> GetUserLikeComment(string CommentId)
        {
            return Ok(await _likeService.GetUserLikePostsOrComments(null, CommentId));
        }
    }
}
