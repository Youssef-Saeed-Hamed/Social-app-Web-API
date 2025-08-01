using Core_Layer;
using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities.Identity;
using Core_Layer.Inetrfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Service_Layer;
using System.Net;
using System.Text;

namespace Graduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        public AccountController(IAccountService accountService, UserManager<AppUser> userManager, IEmailService emailService)
        {
            _accountService = accountService;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Response>> Register(RegisterDto user)
        {
            // make a register
            var response = await _accountService.RegisterAsync(user);
            // after make a register get the user to send an email to confirm his email 
            var Appuser = await _userManager.FindByEmailAsync(user.Email);

            // generate token for confirm email by pass user
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(Appuser);


			var tokenBytes = Encoding.UTF8.GetBytes(token);
			var safeToken = WebEncoders.Base64UrlEncode(tokenBytes);
			var encodedEmail = WebUtility.UrlEncode(Appuser.Email);

			//Create url and frontend developer will make a ui for this url
			//var url = Url.Action(nameof(ConfirmEmail), "Account", new {token = token ,email = Appuser.Email } , Request.Scheme);
			var url = $"{user.ConfirmEmailUrl}?token={safeToken}&email={encodedEmail}";

			var Email = new Email
            {
                Body = url!,
                Subject = "تأكيد بريدك الإلكترونى",
                To = user.Email
            };
            // send an email and its body has url when user click on the email it gnrate end point confirm email
            _emailService.SendEmail(Email);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<ReturnLoginDto>> Login(LoginDto user)
        {
            return user is not null ? Ok(await _accountService.LoginAsync(user)) : BadRequest();
        }
        
        [HttpPost("ChangePassword")]
        public async Task<ActionResult<Response>> ChangePassword(ChangePasswordDto user)
        {
            var response = await _accountService.ChangePasswordAsync(user);

            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token,string email)
        {



            byte[] tokenBytes;
            try
            {
            	tokenBytes = WebEncoders.Base64UrlDecode(token);
            }
            catch
            {
            	return BadRequest("رمزك المميز غير متاح");
            }
            var decodedToken = Encoding.UTF8.GetString(tokenBytes);
            var decodedEmail = WebUtility.UrlDecode(email);


            var response = await _accountService.VerifyEmail(decodedToken, decodedEmail);
            //var response = await _accountService.VerifyEmail(token, email);
            if (response.Status == "Success")
                return StatusCode(StatusCodes.Status200OK, response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
        [HttpPost("ForgetPassword")]
        [AllowAnonymous]
        public async Task<ActionResult<Response>> ForgetPassword(ForgetPasswordDto input)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);

            if (user is not null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var url = Url.Action("ResetPassword", "Account", new { token = token, email = input.Email }, Request.Scheme);
                var Email = new Email
                {
                    To = input.Email,
                    Subject = "تأكيد بريدك الالكترونى لإعادة تعيين كلمة المرور",
                    Body = url!
                };

                _emailService.SendEmail(Email);

                return StatusCode(StatusCodes.Status200OK, new Response
                {
                    Status = "Success",
                    Message = "أفحص بريدك الإلكترونى واضغط على الرابط لتاكيد البريد الإلكترونى"
                });
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response
            {
                Status = "Failed",
                Message = "بريدك الإلكترونى غير موجود"
            });
        }
        [HttpGet("ResetPassword")]
        public ActionResult<ResetPasswordDto> ResetPassword (string token , string email)
        {
            return Ok(new ResetPasswordDto
            {
                Email = email,
                Token = token
            });
        }
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<ActionResult<Response>> ResetPassword(ResetPasswordDto input)
        {
            
            var user = await _userManager.FindByEmailAsync(input.Email);

            if (user is not null)
            {
                var result = await _userManager.ResetPasswordAsync(user, input.Token, input.Password);
                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return Ok(result);
                }

                return StatusCode(StatusCodes.Status200OK, new Response
                {
                    Status = "Success",
                    Message = "تم تغيير كلمة مروررك يمكنك الآن التسجيل بكلمة المرور الجديدة"
                });
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response
            {
                Status = "Failed",
                Message = "كلمة المرور لم تتغير"
            });
        }


    }
}
 