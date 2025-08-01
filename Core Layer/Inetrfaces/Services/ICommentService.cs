using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Inetrfaces.Services
{
    public interface ICommentService
    {
        public Task<Response>InsertCommentAsync(InputCommentDto commentDto, string UserId,string ? parentId = null);
        public Task<ResponseForUpdateAndDeleteDto> UpdateComment(InputCommentDto commentDto, string UserId,string  commentId);
        public Task<ResponseForUpdateAndDeleteDto> DeleteComment(string UserId , string CommentId);
        public Task<IEnumerable< CommentsDto>> GetCommentsAsync(CommentSpecificationParameter parameters , string UserId);
        

    }
}
