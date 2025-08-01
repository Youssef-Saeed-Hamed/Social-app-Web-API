using Core_Layer;
using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities;
using Core_Layer.Entities.Identity;
using Core_Layer.Inetrfaces.Repositries;
using Core_Layer.Inetrfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<AppUser> _signInManger;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public AccountService(SignInManager<AppUser> signInManger, UserManager<AppUser> userManager,
            ITokenService tokenService, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _signInManger = signInManger;
            _userManager = userManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<Response> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return new Response
                {
                    Status = "Failed",
                    Message = "هذا البريد الإلكترونى  غير موجود"
                };

            var result = await _signInManger.CheckPasswordSignInAsync(user, dto.CurrentPassword, false);
            if (!result.Succeeded)
                return new Response
                {
                    Status = "Failed",
                    Message = "خطأ فى البريد الإلكترونى او كلمة المرور "
                };

            var NewPasswordResult = await _userManager.ChangePasswordAsync(user,dto.CurrentPassword , dto.NewPassword);
            if(!NewPasswordResult.Succeeded)
                return new Response
                {
                    Status = "Failed",
                    Message = "حصل خطأ اثناء تغيير كلمة المرور"
                };
            return new Response
            {
                Status = "Success",
                Message = "تم تغيير كلمة مرورك بنجاح ,تقدر تعمل تسجيل دخول بكلمة المرور الجديدة"
            };



        }

        public async Task<ReturnLoginDto> LoginAsync(LoginDto UserDto)
        {
            // get user by his email 
            var user = await _userManager.FindByEmailAsync(UserDto.Email);

            // if user not exist return response this user is not exist 
            if (user is null)
                throw new Exception($" غير موجود {UserDto.Email} هذا البريد الألكترونى ");

            // check that password which user is send it == password for this user
            var result = await _signInManger.CheckPasswordSignInAsync(user, UserDto.Password , false);
            if (!result.Succeeded)
                throw new Exception("حصل خطأ فى البريد الالكترونى او كلمة المرور");
            return new ReturnLoginDto
            {
                Id = user.Id,
                Email = UserDto.Email,
                Name = user.UserName,
                Token = await _tokenService.GenerateToken(user)
            };
        }

        public async Task<Response> RegisterAsync(RegisterDto UserDto )
        {
            // Is there user by this email 
            var userCheck = await _userManager.FindByEmailAsync(UserDto.Email);
            
            // if we already has a user by this email.
            if (userCheck is not null)
            {

                // if his email is not confirmed
                if (!userCheck.EmailConfirmed)
                // this operation because may be user register but not confirm his email
                // we delete this record and make new register
                {

                    // delete this user from security table
                    await _userManager.DeleteAsync(userCheck);

                    // get user from user table
                    var user = await _unitOfWork.Repositry<User, string>().GetAsync(userCheck.Id);
                    // delete user from user table
                    _unitOfWork.Repositry<User, string>().Delete(user);
                    // save changes
                    await _unitOfWork.CompleteAsync();
                    userCheck = null;
                }
                else // is not null and his email was confirmed 

                    throw new Exception($"موجود بالفعل {userCheck.Email} هذا البريد الإلكترونى ");

               

            }
            
            // make an objectb from appUser
            var AppUser = new AppUser
            {               
                Email = UserDto.Email,
                UserName = UserDto.Email.Substring(0 , UserDto.Email.IndexOf('@')),
                IsBlind = UserDto.IsBlind,
            };

            // create an new user in app
            var result = await _userManager.CreateAsync(AppUser , UserDto.Password );
            if (!result.Succeeded)
                throw new Exception("كلمة مرور خطأ");

            
            // make an object from user 
            var User = new User
            {
                Id = AppUser.Id,
                BirthDate = UserDto.BirthDate,
                FirstName = UserDto.FirstName,
                LastName = UserDto.LastName,
                ImageUrl = UserDto.ImageUrl,
                IsBlind = UserDto.IsBlind
                
            };
            // add this user in users tables
            await _unitOfWork.Repositry<User, string>().InsertAsync(User);
            await _unitOfWork.CompleteAsync();

           

            return new Response
            {
                Status = "Success",
                Message = "تم التسجيل بنجاح يجب تأكيد بريدك الإلكترونى , افحص بريدك الإلكترونى"
            };


        }

        public async Task <Response> VerifyEmail(string token, string email)
        {
            // get user by his email 
            var user = await _userManager.FindByEmailAsync(email);
            if(user is not null)
            {
                // confirm the email by pass user and token 
                var result = await _userManager.ConfirmEmailAsync(user, token);
                // confirm is succeeded return response that email is confirmed successfully 
                if (result.Succeeded)
                    return new Response { Status = "Success", Message = "تم تأكيد بريدك الإلكترونى بنجاح" }; 
            }
            // email is not confirmed
            return new Response { Status = "Failed", Message = "لم يتم تأكيد بريدك الالكترونى لوجود خطأ ما" };
        }
    }
}
