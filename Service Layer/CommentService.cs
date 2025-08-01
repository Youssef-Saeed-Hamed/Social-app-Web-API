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
        private readonly IModelsAiService _modelsAiService;
        public CommentService(IUnitOfWork unitOfWork, INotificationService notificationService, IModelsAiService modelsAiService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _modelsAiService = modelsAiService;
        }

        public async Task<ResponseForUpdateAndDeleteDto> DeleteComment(string UserId, string commentId)
        {
            var response = new Response();
            var comment = await _unitOfWork.Repositry<Comment, string>().GetAsync(commentId);
            if (comment is null)
                response =  new Response
                {
                    Status = "Failed",
                    Message = "لا يوجد تعليق بهذا المعرف"
                };

            if (UserId != comment.UserId)
                response =  new Response
                {
                    Status = "Failed",
                    Message = "لا يمكنك مسح هذا التعليق لأنه مش تعليقك"
                };


            _unitOfWork.Repositry<Comment, string>().Delete(comment);
            if (await _unitOfWork.CompleteAsync() <= 0)
                response = new Response
                {
                    Status = "Failed",
                    Message = "حصل خطأ اثناء مسح التعليق"
                };
            else
            {

                response = new Response
                {
                    Status = "Success",
                    Message = "تم مسح التعليق بنجاح"
                };
            }
            return new ResponseForUpdateAndDeleteDto
            {
                Response = response,
                imagePath = comment.imagePath
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
                        ImagePath = string.IsNullOrWhiteSpace(comment.User.ImageUrl) ? "" : comment.User.ImageUrl 
                        
                    },
                    IsLiked = IsLike,
                    NumberOfLikes = LikesLength,
                    RepliesSize = comment.Recives?.Count ?? 0,
                    imagePath = comment.imagePath           
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
                imagePath = commentDto.ImagePath,
                
            };


            
                await _unitOfWork.Repositry<Comment, string>().InsertAsync(comment);
                if (await _unitOfWork.CompleteAsync() <= 0)
                    return new Response
                    {
                        Status = "Failed",
                        Message = "حصل خطأ اثناء إضافة التعليق"
                    };
                if (parentId is null)
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
                    Message = "تم إضافة التعليق بنجاح"
                };
           

        }

        public async Task<ResponseForUpdateAndDeleteDto> UpdateComment(InputCommentDto commentDto, string UserId, string commentId)
        {
            string oldImagePath = "";

            oldImagePath = commentDto.ImagePath ?? "";

            var specs = new CommentSpecification(commentId);
            var response = new Response();
            var comment = await _unitOfWork.Repositry<Comment , string>().GetWithSpecAsync(specs);
            if (comment is null)
                response = new Response
                {
                    Status = "Failed",
                    Message = "لا يوجد تعليق بهذا المعرف"
                };

            if (UserId != comment.UserId)
                response =  new Response
                {
                    Status = "Failed",
                    Message = "حصل خطأ اثناء تعديل ال التعليق لأنه ليس تعليقك"
                };

            comment.Content = commentDto.CommentContent;
            comment.imagePath = commentDto.ImagePath;

           
                _unitOfWork.Repositry<Comment, string>().Update(comment);
            if (await _unitOfWork.CompleteAsync() <= 0)
                response = new Response
                {
                    Status = "Failed",
                    Message = "فشل تعديل هذا التعليق"
                };
            else
            {

                response = new Response
                {
                    Status = "Success",
                    Message = "تم تعديل التعليق بنجاح"
                };
            }

            return new ResponseForUpdateAndDeleteDto
            {
                Response = response,
                imagePath = oldImagePath
            };

        }
    }
}
