using Core_Layer.Data_Transfer_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Inetrfaces.Services
{
    public interface IUserServices
    {
        public Task<IEnumerable<ReturnUsersDto>> GetAllUsers(string UserId);
        public Task<UserProfileDto> GetUserProfile(string Id, string UserId);
        public Task<UserProfileDto> GetMyProfile(string Id);
        public Task<Response> UpdateProfile(InputUpdateProfileDto input , string UserId);
        public Task<UserPostDto> GetUserData(string userId);


    }
}
