using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities;
using Core_Layer.Inetrfaces.Repositries;
using Core_Layer.Inetrfaces.Services;
using Repository_Layer.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer
{
    public class FollowingService : IFollowingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserFollowersRepository _userFollowersRepository;
        private readonly INotificationService _notificationService;

        public FollowingService(IUnitOfWork unitOfWork, IUserFollowersRepository userFollowersRepository, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _userFollowersRepository = userFollowersRepository;
            _notificationService = notificationService;
        }

        //public async Task<ReturnFollowerMessageDto> AcceptFollow(InputFollowerDto input, string UserId)
        //{
        //    var request = await _userFollowersRepository.GetAsync(input.UserId , UserId);
        //    if (request is null)
        //        throw new Exception("No Request Between Yours");

        //    if (request.IsFollowing)
        //        throw new Exception("You Already Follow");

        //    request.IsFollowing = true;
        //    await _notificationService.InsertNotification(input.UserId, UserId, null, $"تم قبول طلب متابعتك");
        //    _userFollowersRepository.Update(request);

        //    if (await _userFollowersRepository.CompleteAsync() <= 0)
        //        throw new Exception("You Can't Update");

        //    return new ReturnFollowerMessageDto
        //    {
        //        Message = "The Request Accepted"
        //    };
        //}

        public async Task<Response> CancelFollow(InputFollowerDto input, string UserId)
        {
            var request = await _userFollowersRepository.GetAsync(input.UserId , UserId);
            if (request is null)
                return new Response
                {
                    Status = "Failed",
                    Message = "You Already Don't Follow Him"
                };            

            _userFollowersRepository.Delete(request);

            if (await _userFollowersRepository.CompleteAsync() <= 0)
                return new Response
                {
                    Status = "Failed",
                    Message = "Your Follow Not Canceled Successfully"
                };


            return new Response
            {
                Status = "Success",
                Message = "Your Follow Canceled Successfully"
            };
        }

        public async Task<Response> CancelFollower(InputFollowerDto input, string UserId)
        {
            var request = await _userFollowersRepository.GetAsync(UserId , input.UserId);
            if (request is null)
                return new Response
                {
                    Status = "Failed",
                    Message = "He Already Doesn't Follow You"
                };

            _userFollowersRepository.Delete(request);

            if (await _userFollowersRepository.CompleteAsync() <= 0)
                return new Response
                {
                    Status = "Failed",
                    Message = "You Can't Canceled His Follow Successfully"
                };


            return new Response
            {
                Status = "Success",
                Message = "You Canceled His Follow Successfully"
            };
        }

        //public async Task<ReturnFollowerMessageDto> CancelRequest(InputFollowerDto input, string UserId)
        //{
        //    var request = await _userFollowersRepository.GetAsync(UserId, input.UserId);
        //    if (request is null)
        //        throw new Exception("No Request Between Yours");

        //    if (request.IsFollowing)
        //        throw new Exception("You Can't Delete");

        //    _userFollowersRepository.Delete(request);

        //    if (await _userFollowersRepository.CompleteAsync() <= 0)
        //        throw new Exception("You Can't Delete");

        //    return new ReturnFollowerMessageDto
        //    {
        //        Message = "The Request Deleted Successfully"
        //    };
        //}

        public async Task<IEnumerable<UserPostDto>> GetFollowers(string UserId)
        {
            var specs = new UserFollowersSpecification("", UserId);

            var followers = await _userFollowersRepository.GetAllWithSpecAsync(specs);

            var MappedFollowers = new List<UserPostDto>();
            foreach(var user in followers)
            {
                MappedFollowers.Add(new UserPostDto
                {
                    UserId = user.Follower.Id,
                    Name = $"{user.Follower.FirstName} {user.Follower.LastName}",
                    ImagePath = string.IsNullOrWhiteSpace(user.Follower.ImageUrl) ? "" :
                        Path.Combine(Directory.GetCurrentDirectory(), @"Files/Images", user.Follower.ImageUrl),
                    
                });
            }

            return MappedFollowers;
            
        }

        public async Task<IEnumerable<UserPostDto>> GetFollowings(string UserId)
        {
            var specs = new UserFollowersSpecification(UserId, "");

            var followers = await _userFollowersRepository.GetAllWithSpecAsync(specs);

            var MappedFollowers = new List<UserPostDto>();
            foreach (var user in followers)
            {
                MappedFollowers.Add(new UserPostDto
                {
                    UserId = user.Following.Id,
                    Name = $"{user.Following.FirstName} {user.Following.LastName}",
                    ImagePath = string.IsNullOrWhiteSpace(user.Following.ImageUrl) ? "" :
                        Path.Combine(Directory.GetCurrentDirectory(), @"Files/Images", user.Following.ImageUrl),
                });
            }

            return MappedFollowers;
        }

        //public async Task<IEnumerable<UserPostDto>> GetRequstsISend(string UserId)
        //{

        //    var specs = new UserFollowersSpecification(UserId, "" , false);

        //    var followers = await _userFollowersRepository.GetAllWithSpecAsync(specs);
        //    var MappedFollowers = new List<UserPostDto>();
        //    foreach (var user in followers)
        //    {
        //        MappedFollowers.Add(new UserPostDto
        //        {
        //            UserId = user.Following.Id,
        //            Name = $"{user.Following.FirstName} {user.Following.LastName}",
        //            ImagePath = string.IsNullOrWhiteSpace(user.Following.ImageUrl) ? "" :
        //                Path.Combine(Directory.GetCurrentDirectory(), @"Files/Images", user.Following.ImageUrl),

        //        });
        //    }

        //    return MappedFollowers;
        //}

        //public async Task<IEnumerable<UserPostDto>> GetRequstsSendToMe(string UserId)
        //{
        //    var specs = new UserFollowersSpecification("", UserId, false);

        //    var followers = await _userFollowersRepository.GetAllWithSpecAsync(specs);
        //    var MappedFollowers = new List<UserPostDto>();
        //    foreach (var user in followers)
        //    {
        //        MappedFollowers.Add(new UserPostDto
        //        {
        //            UserId = user.Follower.Id,
        //            Name = $"{user.Follower.FirstName} {user.Follower.LastName}",
        //            ImagePath = string.IsNullOrWhiteSpace(user.Follower.ImageUrl) ? "" :
        //                Path.Combine(Directory.GetCurrentDirectory(), @"Files/Images", user.Follower.ImageUrl),
        //        });
        //    }

        //    return MappedFollowers;
        //}

        //public async Task<ReturnFollowerMessageDto> RefuseFollow(InputFollowerDto input, string UserId)
        //{


        //    var request = await _userFollowersRepository.GetAsync(input.UserId , UserId);
        //    if (request is null)
        //        throw new Exception("No Request Between Yours");

        //    if (request.IsFollowing)
        //        throw new Exception("You Already Not Follow");


        //    _userFollowersRepository.Delete(request);

        //    if (await _userFollowersRepository.CompleteAsync() <= 0)
        //        throw new Exception("You Can't Refuse");

        //    return new ReturnFollowerMessageDto
        //    {
        //        Message = "The Request Refused"
        //    };
        //}

        public async Task<Response> SendFollow(InputFollowerDto input, string userId)
        {
            if (await _unitOfWork.Repositry<User, string>().GetAsync(input.UserId) is null)
                return new Response
                {
                    Status = "Failed",
                    Message = "There Isn't User With This Id"
                };

            if (await _userFollowersRepository.GetAsync(userId , input.UserId) is not null)
                return new Response
                {
                    Status = "Failed",
                    Message = "You Already Follow Him"
                };

            var followUser = new UserFollowers
            {
                FollowerId = userId,
                FollowingId = input.UserId,
                DateTime = DateTime.Now,
                IsFollowing = true
            };

            await _userFollowersRepository.InsertAsync(followUser);

            if (await _userFollowersRepository.CompleteAsync() < 0)
                return new Response
                {
                    Status = "Failed",
                    Message = "Your Requst Not Sended Successfully"
                };
            await _notificationService.InsertNotification(input.UserId, userId, null, $"ارسل اليك طلب متابعة");


            return new Response
            {
                Status = "Success",
                Message = "You Follow Him Now Successfully"
            };


        }
    }
}
