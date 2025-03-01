using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities;
using Core_Layer.Inetrfaces.Repositries;
using Core_Layer.Inetrfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer
{
    public class UserService : IUserServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFollowingService _followingService;
        private readonly IPostService _postService;

        public UserService(IUnitOfWork unitOfWork, IFollowingService followingService, IPostService postService)
        {
            _unitOfWork = unitOfWork;
            _followingService = followingService;
            _postService = postService;
        }

        public async Task<IEnumerable<ReturnUsersDto>> GetAllUsers(string UserId)
        {
            var users = await _unitOfWork.Repositry<User , string >().GetAllAsync();
            
            var MappedUsers = new List<ReturnUsersDto>();
            foreach (var user in users)
            {
                if (user.Id == UserId)
                    continue;
                MappedUsers.Add(new ReturnUsersDto
                {
                    Id = user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                    ImageUrl = string.IsNullOrWhiteSpace(user.ImageUrl) ? "" :
                         Path.Combine(Directory.GetCurrentDirectory(), @"Files/Images", user.ImageUrl),
                });
            }
                

            return MappedUsers;
            
        }

        public async Task<UserProfileDto> GetMyProfile(string Id)
        {
            var user = await _unitOfWork.Repositry<User , string>().GetAsync(Id);
            if (user is null)
                throw new Exception("You Are Not Authorized");

            return new UserProfileDto
            {
                Id = user.Id,
                Name = $"{user.FirstName} {user.LastName}",
                ImagePath = string.IsNullOrWhiteSpace(user.ImageUrl) ? "" :
                            Path.Combine(Directory.GetCurrentDirectory(), @"Files/Images", user.ImageUrl),
                BirthDate = user.BirthDate,
                Posts = await _postService.GetAllPosts(new PostSpecificationParameters{ UserId = user.Id} ,Id )
            };
        }

        public async Task<UserProfileDto> GetUserProfile(string Id , string UserId )
        {
            var Followings = await _followingService.GetFollowings(Id);
            var User = await _unitOfWork.Repositry<User , string>().GetAsync(UserId);

            if (User is null)
                throw new Exception("No User With Id ");
            UserProfileDto? userProfile = null;
            foreach(var user in Followings)
            {
                if (user.UserId == UserId)
                {
                    userProfile = new UserProfileDto
                    {
                        Id = user.UserId,
                        Name = user.Name,
                        ImagePath = string.IsNullOrWhiteSpace(user.ImagePath) ? "" :
                            Path.Combine(Directory.GetCurrentDirectory(), @"Files/Images", user.ImagePath),
                        BirthDate = User.BirthDate,
                        Posts = await _postService.GetAllPosts(new PostSpecificationParameters { UserId = user.UserId } , Id),
                    };
                }
                    
            }

            if (userProfile is null)
                throw new Exception("You Don't Follow This User");

            return userProfile;
        }
        public async Task<UserPostDto> GetUserData(string userId)
        {
            var user = await _unitOfWork.Repositry<User, string>().GetAsync(userId);
            if (user is null)
                throw new Exception("You Are Not Authorized");
            return new UserPostDto
            {
                UserId = user.Id,
                Name = $"{user.FirstName} {user.LastName}",
                ImagePath = user.ImageUrl ?? "",
            };
        }

        public async Task<Response> UpdateProfile(InputUpdateProfileDto input , string UserId)
        {
            var user = await _unitOfWork.Repositry<User , string>().GetAsync(UserId);
            if (user is null)
                return new Response
                {
                    Status = "Failed",
                    Message = "You Are Not Authorized"
                };

            user.FirstName = input.FirstName;
            user.LastName = input.LastName;
            user.BirthDate =input.BirthDate;
            user.IsBlind = input.IsBlind;

            _unitOfWork.Repositry<User , string>().Update(user);
            if (await _unitOfWork.CompleteAsync() <= 0)
                return new Response
                {
                    Status = "Failed",
                    Message = "Your Data Didn't Updated Successfully"
                };
            return new Response
            {
                Status = "Success",
                Message = "Your Data Updated Successfully"
            };

        }
    }
}
