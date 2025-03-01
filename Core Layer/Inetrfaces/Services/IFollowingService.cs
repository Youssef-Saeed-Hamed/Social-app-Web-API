using Core_Layer.Data_Transfer_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Inetrfaces.Services
{
    public interface IFollowingService
    {
        public Task<Response> SendFollow(InputFollowerDto input , string UserId);
        //public Task<ReturnFollowerMessageDto> CancelRequest(InputFollowerDto input , string UserId);
        public Task<Response> CancelFollow(InputFollowerDto input, string UserId);
        public Task<Response> CancelFollower(InputFollowerDto input, string UserId);
        //public Task<ReturnFollowerMessageDto> AcceptFollow(InputFollowerDto input, string UserId);
        //public Task<ReturnFollowerMessageDto> RefuseFollow(InputFollowerDto input, string UserId);

        public Task<IEnumerable< UserPostDto>> GetFollowers(string UserId);
        public Task<IEnumerable< UserPostDto>> GetFollowings(string UserId);
        //public Task<IEnumerable< UserPostDto>> GetRequstsSendToMe(string UserId);
        //public Task<IEnumerable< UserPostDto>> GetRequstsISend(string UserId);
    }
}
