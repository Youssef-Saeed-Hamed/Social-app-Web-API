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
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IModelsAiService _modelsAiService;
        private readonly UserManager<AppUser> userManager;

        public PostController(IPostService postService, IModelsAiService modelsAiService, UserManager<AppUser> userManager)
        {
            _postService = postService;
            _modelsAiService = modelsAiService;
            this.userManager = userManager;
        }
        [HttpPost]
        public async Task<ActionResult<PostDto>> AddPost([FromForm] PostInputDto dto )
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            
            var user = await userManager.FindByIdAsync(UserId);
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

            var postContentStatus = await _modelsAiService.AnalyzeTextAsync(dto.Content , user);
            if (postContentStatus == "Bullying")
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Failed",
                    Message = "لا يمكنك نشر هذا المنشور لانه يحتوى على كلام غير أخلاقى"
                });
            }

            string extractedText = string.Empty;
            
            string imagePath = "";
            if (dto.Image is not null)
            {
                using var ms = new MemoryStream();
                await dto.Image.CopyToAsync(ms);
                var imageBytes = ms.ToArray();
                
                extractedText = await _modelsAiService.ExtractTextFromImageAsync(imageBytes, dto.Image.FileName);
                if (!string.IsNullOrWhiteSpace(extractedText))
                {
                    string status = await _modelsAiService.AnalyzeTextAsync(extractedText, user);
                    if (status == "Bullying")
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response
                        {
                           Status = "Failed",
                            Message = "لا يمكنك نشر هذا المنشور لان الصورة تحتوى على كلام غير أخلاقى"
                        });

                    }
                }
                
                imagePath = ImageSetting.Upload(dto.Image);

            }
            var postDto = new InputPostDto { Content = dto.Content , ImagePath = imagePath };
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
                return BadRequest("معرفك خطأ");
            return Ok(await _postService.GetAllPosts(specs , UserId));
        }
        [HttpGet("Followings")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetFollowingPosts()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return BadRequest("معرفك خطأ");
            return Ok(await _postService.GetAllPostsForMyFollowings(UserId));
        }
        [HttpPost("{Id}")]
        public async Task<ActionResult<PostDto>> UpdatePost([FromForm]InputUpdatePostDto input, string Id)
        {

            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userManager.FindByIdAsync(UserId);
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
            var postContentStatus = await _modelsAiService.AnalyzeTextAsync(input.NewContent , user);
            if (postContentStatus == "Bullying")
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Failed",
                    Message = "لا يمكنك  تعديل هذا المنشور لانه يحتوى على كلام غير أخلاقى"
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
                            Message = "لا يمكنك  تعديل هذا المنشور لان الصورة تحتوى على كلام غير أخلاقى"
                        });

                    }
                }

                imagePath = ImageSetting.Upload(input.Image);

            }
            var dto = new UpdatePostDto { NewContent = input.NewContent  , imagePath = imagePath };
            var response = await _postService.UpdatePost(dto, Id, UserId);
            if (response.Response.Status == "Success")
            {
                if (!string.IsNullOrWhiteSpace(response.imagePath))
                    ImageSetting.Delete(response.imagePath);
                return StatusCode(StatusCodes.Status200OK, response.Response);

            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Response);
        }

        [HttpDelete("{postId}")]
        public async Task<ActionResult<Response>> DeletePost(string postId)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userManager.FindByIdAsync(UserId);
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
            var dto = new DeletePostDto { PostId = postId };
            var response = await _postService.DeletePost(dto, UserId);
            if (response.Response.Status == "Success")
            {
                if (!string.IsNullOrWhiteSpace(response.imagePath))
                    ImageSetting.Delete(response.imagePath);

                return StatusCode(StatusCodes.Status200OK, response.Response);

            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Response);

        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<PostDto>> GetPost(string Id)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok(await _postService.GetPost(Id , UserId!));

        }
    }
}
