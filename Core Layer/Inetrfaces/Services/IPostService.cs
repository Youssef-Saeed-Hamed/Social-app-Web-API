using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Inetrfaces.Services
{
    public interface IPostService
    {
        public Task<Response> AddPost(InputPostDto inputPost , string UserId);
        public Task<ResponseForUpdateAndDeleteDto> UpdatePost(UpdatePostDto inputPost, string PostId,  string UserId);
        public Task<ResponseForUpdateAndDeleteDto> DeletePost(DeletePostDto deletePost , string UserId);
        public Task<PostDto> GetPost(string Id , string UserId);
        public Task<IEnumerable<PostDto>> GetAllPosts(PostSpecificationParameters specsParameters , string UserId);
        public Task<IEnumerable<PostDto>> GetAllPostsForMyFollowings(string UserId);
    }
}
