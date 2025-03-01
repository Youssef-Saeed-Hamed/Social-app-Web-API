using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities;
using Core_Layer.Inetrfaces.Repositries;
using Core_Layer.Inetrfaces.Services;
using Graduation_Project.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Graduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImageController( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<ActionResult<Response>> Upload([FromForm]ImageInputDto imageDto)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "You Are Not Authoized"
                });
            var user = await _unitOfWork.Repositry<User, string>().GetAsync(UserId);
            string message = "";
            if (string.IsNullOrWhiteSpace(user.ImageUrl))
            {
                user.ImageUrl = ImageSetting.Upload(imageDto.Image);
                message = "Your Image Added Successfully";
            }
            else
            {
                ImageSetting.Delete(user.ImageUrl);
                user.ImageUrl = ImageSetting.Upload(imageDto.Image);
                message = "Your Image Updated Successfully";
            }
            _unitOfWork.Repositry<User, string>().Update(user);
            await _unitOfWork.CompleteAsync();
            return new Response
            {
                Status = "Success",
                Message = message
            };
        }
        [HttpDelete]
        public async Task<ActionResult<Response>> Delete()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "You Are Not Authoized"
                });
            var user = await _unitOfWork.Repositry<User, string>().GetAsync(UserId);
            
            if (string.IsNullOrWhiteSpace(user.ImageUrl))
            {
                return Unauthorized(new Response
                {
                    Status = "Failed",
                    Message = "You Already Don't Has an Image"
                });
            }
            else
            {
                ImageSetting.Delete(user.ImageUrl);
                user.ImageUrl = "";
            }
            _unitOfWork.Repositry<User, string>().Update(user);
            await _unitOfWork.CompleteAsync();
            return new Response
            {
                Status ="Success",
                Message = "Your Image Deleted Successfully"
            };
        }
    }
}
