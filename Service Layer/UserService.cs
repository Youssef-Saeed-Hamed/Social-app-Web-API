using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities;
using Core_Layer.Inetrfaces.Repositries;
using Core_Layer.Inetrfaces.Services;
using Repository_Layer.Repositries;
using Repository_Layer.Specifications;
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
        private readonly IUserFollowersRepository _userFollowersRepository;

        private readonly IPostService _postService;

        public UserService(IUnitOfWork unitOfWork, IFollowingService followingService, IPostService postService, IUserFollowersRepository userFollowersRepository)
        {
            _unitOfWork = unitOfWork;
            _followingService = followingService;
            _postService = postService;
            _userFollowersRepository = userFollowersRepository;
        }

        public async Task<IEnumerable<ReturnUsersDto>> GetAllUsers(string UserId)
        {
            var users = await _unitOfWork.Repositry<User , string >().GetAllAsync();
            var followings = await _followingService.GetFollowings(UserId);
            var MappedUsers = new List<ReturnUsersDto>();
            foreach (var user in users)
            {
                if (user.Id == UserId)
                    continue;

                bool isFollowing = false;
                foreach (var follower in followings)
                {
                    if(user.Id == follower.UserId)
                    {
                        isFollowing = true;
                        break;
                    }
                    
                   
                   
                }
                if (!isFollowing)
                {
                    MappedUsers.Add(new ReturnUsersDto
                    {
                        Id = user.Id,
                        Name = $"{user.FirstName} {user.LastName}",
                        ImageUrl = user.ImageUrl ?? ""
                    });


                }
                
            }

                

            return MappedUsers;
            
        }

        public async Task<UserProfileDto> GetMyProfile(string Id , string email)

        {
            var specs = new UserFollowersSpecification("", Id);
            var specs2 = new UserFollowersSpecification(Id, "");


            var followers = await _userFollowersRepository.GetAllWithSpecAsync(specs);
            var followings = await _userFollowersRepository.GetAllWithSpecAsync(specs2);

            var user = await _unitOfWork.Repositry<User , string>().GetAsync(Id);
            if (user is null)
                throw new Exception("ليس لديك صلاحية الوصول");

            return new UserProfileDto
            {
                Email = email,
                IsBlind = user.IsBlind,
                Id = user.Id,
                Name = $"{user.FirstName} {user.LastName}",
                ImagePath = string.IsNullOrWhiteSpace(user.ImageUrl) ? "" : user.ImageUrl,
                BirthDate = user.BirthDate,
                Posts = await _postService.GetAllPosts(new PostSpecificationParameters { UserId = user.Id }, Id),
                numberOfFollowers =  followers.Count(),
                numberOfFollowings = followings.Count()
                
            };
        }

        public async Task<UserProfileDto> GetUserProfile(string Id , string UserId )
        {
            //var Followings = await _followingService.GetFollowings(Id);
            var user = await _unitOfWork.Repositry<User , string>().GetAsync(UserId);

            var specs = new UserFollowersSpecification("", UserId);
            var specs2 = new UserFollowersSpecification(UserId, "");


            var followers = await _userFollowersRepository.GetAllWithSpecAsync(specs);
            var followings = await _userFollowersRepository.GetAllWithSpecAsync(specs2);

            if (user is null)
                throw new Exception("لا يوجد مستخدم بهذا المعرف ");
            UserProfileDto userProfile =  new UserProfileDto() ;

            userProfile = new UserProfileDto
            {
                Id = user.Id,
                Name = $"{user.FirstName} {user.LastName}",
                ImagePath = string.IsNullOrWhiteSpace(user.ImageUrl) ? "" : user.ImageUrl,

                BirthDate = user.BirthDate,
                Posts = await _postService.GetAllPosts(new PostSpecificationParameters { UserId = user.Id }, Id),
                IsBlind = user.IsBlind,
                numberOfFollowers = followers.Count(),
                numberOfFollowings = followings.Count()
            };

                
                   
            

            

            return userProfile;
        }
        public async Task<UserPostDto> GetUserData(string userId)
        {
            var user = await _unitOfWork.Repositry<User, string>().GetAsync(userId);
            if (user is null)
                throw new Exception("ليس لديك صلاحية الوصول");
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
                    Message = "ليس لديك صلاحية الوصول"
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
                    Message = "حصل خطأ اثناء تعديل معلوماتك"
                };
            return new Response
            {
                Status = "Success",
                Message = "تم تعديل معلوماتك بنجاح"
            };

        }
    }
}
