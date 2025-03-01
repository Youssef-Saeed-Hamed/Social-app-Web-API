using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities;
using Core_Layer.Inetrfaces.Repositries;
using Core_Layer.Inetrfaces.Services;
using Repository_Layer.Specifications;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public CommentService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<Response> DeleteComment(string UserId, string commentId)
        {

            var comment = await _unitOfWork.Repositry<Comment, string>().GetAsync(commentId);
            if (comment is null)
                return new Response
                {
                    Status = "Failed",
                    Message = "No Comment With This Id"
                };

            if (UserId != comment.UserId)
                return new Response
                {
                    Status = "Failed",
                    Message = "You Can't Delete This Comment Because That Is Not Your Comment"
                };


            _unitOfWork.Repositry<Comment, string>().Delete(comment);
            if (await _unitOfWork.CompleteAsync() <= 0)
                return new Response
                {
                    Status = "Failed",
                    Message = "The Comment is Not Deleted Successfully"
                };

            return new Response
            {
                Status = "Success",
                Message = "The Comment Is Deleted Successfully"
            };
        }
     
        public async Task<IEnumerable<CommentsDto>> GetCommentsAsync(CommentSpecificationParameter parameters , string UserId) 
        {
            var specs = new CommentSpecification(parameters);
            var comments = await _unitOfWork.Repositry<Comment , string>().GetAllWithSpecAsync(specs);
            var MappedComments = new List<CommentsDto>();
            foreach (var comment in comments)
            {
                var UserLikes = await _unitOfWork.Repositry<LikeComment, string>().GetAllAsync();
                var user = UserLikes.FirstOrDefault(x => x.UserId == UserId && x.CommentId == comment.Id);
                int LikesLength = UserLikes.Count(x => x.CommentId == comment.Id);
                bool IsLike = false;
                if (user is not null )
                    IsLike = true;

                var MappedComment = new CommentsDto
                {
                    CommentId = comment.Id,
                    CommentText = comment.Content,
                    DateTime = comment.DateTime,
                    User = new UserPostDto
                    {
                        UserId = comment.User.Id,
                        Name = $"{comment.User.FirstName} {comment.User.LastName}",
                        ImagePath = string.IsNullOrWhiteSpace(comment.User.ImageUrl) ? "" :
                         Path.Combine(Directory.GetCurrentDirectory(), @"Files/Images", comment.User.ImageUrl , UserId)
                        
                    },
                    IsLiked = IsLike,
                    NumberOfLikes = LikesLength,
                    RepliesSize = comment.Recives?.Count ?? 0,
                };

                MappedComments.Add(MappedComment);
            }

            return MappedComments;

        }

        public async  Task<Response> InsertCommentAsync(InputCommentDto commentDto , string UserId , string? parentId )
        {
            var user = await _unitOfWork.Repositry<User, string>().GetAsync(UserId);
            var comment = new Comment
            {
                Id = Guid.NewGuid().ToString(),
                Content = commentDto.CommentContent,
                PostId = commentDto.PostId,
                UserId = UserId,
                User = user,
                commentParentId = parentId,
                DateTime = DateTime.Now,
                
            };

            await _unitOfWork.Repositry<Comment , string>().InsertAsync(comment);
            if (await _unitOfWork.CompleteAsync() <= 0)
                return new Response
                {
                    Status = "Failed",
                    Message = "The Comment Is Not Added Successfully"
                };
            if(parentId is null)
            {
                var specs = new PostSpecification(commentDto.PostId);
                var post = await _unitOfWork.Repositry<Post, string>().GetWithSpecAsync(specs);
                await _notificationService.InsertNotification(post.UserId, UserId, post.Id, $"علق على منشور لك");

            }
            else
            {
                var specs = new CommentSpecification(parentId);
                var Comment = await _unitOfWork.Repositry<Comment, string>().GetWithSpecAsync(specs);
                await _notificationService.InsertNotification(Comment.UserId, UserId, Comment.PostId, $"علق على تعليق لك");
                

            }

            return new Response
            {
                Status = "Success",
                Message = "The Comment Is Added Successfully"
            };
        }

        public async Task<Response> UpdateComment(InputCommentDto commentDto, string UserId, string commentId)
        {
            var specs = new CommentSpecification(commentId);

            var comment = await _unitOfWork.Repositry<Comment , string>().GetWithSpecAsync(specs);
            if (comment is null)
                return new Response
                {
                    Status = "Failed",
                    Message = "No Comment With This Id"
                };

            if (UserId != comment.UserId)
                return new Response
                {
                    Status = "Failed",
                    Message = "You Can't Delete This Comment Because That Is Not Your Comment"
                };

            comment.Content = commentDto.CommentContent;

            _unitOfWork.Repositry<Comment, string>().Update(comment);
            if (await _unitOfWork.CompleteAsync() <= 0)
                return new Response
                {
                    Status = "Failed",
                    Message = "The Comment is Not Updated Successfully"
                };

            return new Response
            {
                Status = "Success",
                Message = "The Comment Is Updated Successfully"
            };
        }
    }
}
