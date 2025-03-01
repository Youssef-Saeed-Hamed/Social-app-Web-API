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
    public class LikeService : ILikeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public LikeService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<Response> AddLike(string? PostId, string? CommentId, string UserId)
        {
            
            if (!string.IsNullOrWhiteSpace(CommentId))
            {
                var like = new LikeComment
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = UserId,
                    CommentId = CommentId
                };
                
                await _unitOfWork.Repositry<LikeComment , string>().InsertAsync(like);
                await _unitOfWork.CompleteAsync();
                var comment = await _unitOfWork.Repositry<Comment, string>().GetWithSpecAsync(new CommentSpecification(CommentId));
                await _notificationService.InsertNotification(comment.UserId, UserId, PostId, $"اعجب ب منشور لك");
                return new Response { Status = "Success", Message = "You Like This Comment Successfully" };
            }

            else if (!string.IsNullOrWhiteSpace(PostId))
            {
                var like = new LikePost
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = UserId,
                    PostId = PostId
                };

                await _unitOfWork.Repositry<LikePost, string>().InsertAsync(like);
                await _unitOfWork.CompleteAsync();
                var Post = await _unitOfWork.Repositry<Post, string>().GetWithSpecAsync(new PostSpecification(PostId));
                await _notificationService.InsertNotification(Post.UserId, UserId, PostId, $"اعجب ب منشور لك");

                return new Response { Status = "Success", Message = "You Like This Post Successfully" };
            }
            return new Response { Status = "Failed", Message = "Id is Required" };

        }

        public async Task<Response> CancelLike(string? PostId, string? CommentId, string UserId )
        {
            
            if (!string.IsNullOrWhiteSpace(CommentId))
            {
                var like = (await _unitOfWork.Repositry<LikeComment, string>().GetAllAsync())
                    .FirstOrDefault(x => x.UserId == UserId && x.CommentId == CommentId );

                if (like is null || like.UserId != UserId)
                    return new Response { Status = "Failed", Message = "You Already Don't Like" };

                 _unitOfWork.Repositry<LikeComment, string>().Delete(like);
                await _unitOfWork.CompleteAsync();
                return new Response { Status = "Success", Message = "You Dislike This Comment Successfully" };
            }

            else if (!string.IsNullOrWhiteSpace(PostId))
            {
                var like = (await _unitOfWork.Repositry<LikePost, string>().GetAllAsync())
                    .FirstOrDefault(x => x.UserId == UserId && x.PostId == PostId);
                if (like is null || like.UserId != UserId)
                    return new Response { Status = "Failed", Message = "You Already Don't Like" };

                _unitOfWork.Repositry<LikePost, string>().Delete(like);
                await _unitOfWork.CompleteAsync();
                return new Response { Status = "Success", Message = "You Dislike This Post Successfully" };
            }
            return new Response { Status = "Failed", Message = "Id is Required" };
        }

        public async Task<IEnumerable<UserPostDto>> GetUserLikePostsOrComments(string? PostId, string? CommentId)
        {
            if (!string.IsNullOrWhiteSpace(CommentId))
            {
                var likes = await _unitOfWork.Repositry<LikeComment, string>().GetAllWithSpecAsync(new LikeCommentSpecification());
                var MappedLikes = new List<UserPostDto>();
                foreach(var like in likes)
                {
                    MappedLikes.Add(new UserPostDto
                    {
                        UserId = like.UserId,
                        Name = $"{like.User.FirstName} {like.User.LastName}",
                        ImagePath = string.IsNullOrWhiteSpace(like.User.ImageUrl) ? "" :
                            Path.Combine(Directory.GetCurrentDirectory(), @"Files/Images", like.User.ImageUrl),
                    });
                }
                return MappedLikes;
            }

            else if (!string.IsNullOrWhiteSpace(PostId))
            {
                var likes = await _unitOfWork.Repositry<LikePost, string>().GetAllWithSpecAsync(new LikePostSpecification());
                var MappedLikes = new List<UserPostDto>();
                foreach (var like in likes)
                {
                    MappedLikes.Add(new UserPostDto
                    {
                        UserId = like.UserId,
                        Name = $"{like.User.FirstName} {like.User.LastName}",
                        ImagePath = string.IsNullOrWhiteSpace(like.User.ImageUrl) ? "" :
                            Path.Combine(Directory.GetCurrentDirectory(), @"Files/Images", like.User.ImageUrl),
                    });
                }
                return MappedLikes;
            }

            return new List<UserPostDto>(); 
        }
    }
}
