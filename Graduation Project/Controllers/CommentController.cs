using Core_Layer;
using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities;
using Core_Layer.Entities.Identity;
using Core_Layer.Inetrfaces.Services;
using Graduation_Project.dtos;
using Graduation_Project.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly IModelsAiService _modelsAiService;
        private readonly UserManager<AppUser> _userManger;

        public CommentController(ICommentService commentService, IModelsAiService modelsAiService, UserManager<AppUser> userManger)
        {
            _commentService = commentService;
            _modelsAiService = modelsAiService;
            _userManger = userManger;
        }

        [HttpPost]
        public async Task<ActionResult<Response>> Add([FromForm] CommentInputDto input)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManger.FindByIdAsync(UserId);
            if (user.PunishedUntil.HasValue && user.PunishedUntil > DateTime.UtcNow)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                   new Response
                                   {
                                       Status = "Failed",
                                       Message = "لا يمكنك التفاعل الآن بسبب 5 مخالفات متتالية. يمكنك التفاعل بعد 24 ساعة."
                                   });
            }
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "ليس لديك صلاحية الوصول"
                });
            var postContentStatus = await _modelsAiService.AnalyzeTextAsync(input.Content, user);
            if (postContentStatus == "Bullying")
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Failed",
                    Message = "لا يمكنك نشر هذا التعليق لانه يحتوى على كلام غير أخلاقى"
                });
            }
            string extractedText = string.Empty;

            string imagePath = "";
            if (input.Image is not null)
            {
                using var ms = new MemoryStream();
                await input.Image.CopyToAsync(ms);
                var imageBytes = ms.ToArray();
                
                extractedText = await _modelsAiService.ExtractTextFromImageAsync(imageBytes, input.Image.FileName);
                if (!string.IsNullOrWhiteSpace(extractedText))
                {
                    string status = await _modelsAiService.AnalyzeTextAsync(extractedText , user);
                    if (status == "Bullying")
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response
                        {
                            Status = "Failed",
                            Message = "لا يمكنك نشر هذا التعليق لان الصورة تحتوى على كلام غير أخلاقى"
                        });

                    }
                }

                imagePath = ImageSetting.Upload(input.Image);

            }
            var commentDto = new InputCommentDto { CommentContent = input.Content , PostId = input.PostId , ImagePath = imagePath };
            var response = await _commentService.InsertCommentAsync(commentDto, UserId);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
        [HttpPost("{ParentCommentId}")]
        public async Task<ActionResult<Response>> Add( InputCommentDto commentDto , string ParentCommentId)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "ليس لديك صلاحية الوصول"
                });

            var response = await _commentService.InsertCommentAsync(commentDto, UserId, ParentCommentId);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            
        }
        [HttpPut("{CommentId}")]
        public async Task<ActionResult<Response>> Update([FromForm]CommentInputDto input , string CommentId)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManger.FindByIdAsync(UserId);
            if (user.PunishedUntil.HasValue && user.PunishedUntil > DateTime.UtcNow)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                   new Response
                                   {
                                       Status = "Failed",
                                       Message = "لا يمكنك التفاعل الآن بسبب 5 مخالفات متتالية. يمكنك التفاعل بعد 24 ساعة."
                                   });
            }
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "ليس لديك صلاحية الوصول"
                });
            var postContentStatus = await _modelsAiService.AnalyzeTextAsync(input.Content, user);
            if (postContentStatus == "Bullying")
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Failed",
                    Message = "لا يمكنك  تعديل هذا التعليق لانه يحتوى على كلام غير أخلاقى"
                });
            }
            string extractedText = string.Empty;

            string imagePath = "";
            if (input.Image is not null)
            {
                using var ms = new MemoryStream();
                await input.Image.CopyToAsync(ms);
                var imageBytes = ms.ToArray();
                
                extractedText = await _modelsAiService.ExtractTextFromImageAsync(imageBytes, input.Image.FileName);
                if (!string.IsNullOrWhiteSpace(extractedText))
                {
                    string status = await _modelsAiService.AnalyzeTextAsync(extractedText , user);
                    if (status == "Bullying")
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response
                        {
                            Status = "Failed",
                            Message = "لا يمكنك  تعديل هذا التعليق لان الصورة تحتوى على كلام غير أخلاقى"
                        });

                    }
                }

                imagePath = ImageSetting.Upload(input.Image);

            }
            var commentDto = new InputCommentDto { CommentContent = input.Content , ImagePath = imagePath , PostId = input.PostId };
            var response = await _commentService.UpdateComment(commentDto, UserId, CommentId);
            if (response.Response.Status == "Success")
            {

                if (!string.IsNullOrWhiteSpace(response.imagePath))
                    ImageSetting.Delete(response.imagePath);

                return StatusCode(StatusCodes.Status200OK, response.Response);
            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpDelete("CommentId")]
        public async Task<ActionResult<Response>> Delete(string CommentId)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManger.FindByIdAsync(UserId);
            if (user.PunishedUntil.HasValue && user.PunishedUntil > DateTime.UtcNow)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                   new Response
                                   {
                                       Status = "Failed",
                                       Message = "لا يمكنك التفاعل الآن بسبب 5 مخالفات متتالية. يمكنك التفاعل بعد 24 ساعة."
                                   });
            }
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "ليس لديك صلاحية الوصول"
                });
            var response = await _commentService.DeleteComment(UserId, CommentId);
            if (response.Response.Status == "Success")
            {
                if (!string.IsNullOrWhiteSpace(response.imagePath))
                    ImageSetting.Delete(response.imagePath);
                return StatusCode(StatusCodes.Status200OK, response.Response);

            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentsDto>>> GetAll([FromQuery] CommentSpecificationParameter parameters)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return BadRequest("معرفك خطأ");
            return Ok(await _commentService.GetCommentsAsync(parameters , UserId));
        }

    }
}
