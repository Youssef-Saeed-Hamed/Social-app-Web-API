using Core_Layer.Data_Transfer_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Inetrfaces.Services
{
    public interface ILikeService
    {
        public Task<Response> AddLike(string? PostId, string? CommentId , string UserId);
        public Task<Response> CancelLike(string? PostId, string? CommentId, string UserId);
        public Task<IEnumerable<UserPostDto>> GetUserLikePostsOrComments(string? PostId, string? CommentId);

        
    }
}
