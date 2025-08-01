using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities;
using Core_Layer.Inetrfaces.Repositries;
using Core_Layer.Inetrfaces.Services;
using Microsoft.EntityFrameworkCore;
using Repository_Layer.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILikeService _likeService;
        private readonly ICommentService _commentService;
        private readonly IFollowingService _followingService;
        public PostService(IUnitOfWork unitOfWork, ILikeService likeService, ICommentService commentService, IFollowingService followingService, IModelsAiService modelsAiService)
        {
            _unitOfWork = unitOfWork;
            _likeService = likeService;
            _commentService = commentService;
            _followingService = followingService;
        }

        public async Task<Response> AddPost(InputPostDto inputPost , string UserId)
        {
            var user = await _unitOfWork.Repositry<User, string>().GetAsync(UserId);
            var post = new Post
            {
                Id = Guid.NewGuid().ToString(),
                Content = inputPost.Content,
                User = user,
                UserId = UserId,
                DateTime = DateTime.Now,
                imagePath = inputPost.ImagePath,
            };
            
            await _unitOfWork.Repositry<Post, string>().InsertAsync(post);
            if (await _unitOfWork.CompleteAsync() <= 0)
                return new Response
                {
                    Status = "Failed",
                    Message = "حصل خطأ اثناء إضافة منشورك"
                };

            return new Response
            {
                Status = "Success",
                Message = "تم إضافة منشورك بنجاح"
            };
            
            
                
        }

        public async Task<ResponseForUpdateAndDeleteDto> DeletePost(DeletePostDto deletePost, string UserId)
        {
            Response response = new Response();
            
            var specs = new PostSpecification(deletePost.PostId);
            var post = await _unitOfWork.Repositry<Post, string>().GetWithSpecAsync(specs);
            if (post is null)
                 response =  new Response
                {
                    Status = "Failed",
                     Message = "لا يوجد منشور بهذا المعرف"
                 };

            if (UserId != post.UserId)
                response = new Response
                {
                    Status = "Failed",
                    Message = "لا يمكنك مسح هذا المنشور"
                };

            // حذف اللايكات المرتبطة بالبوست
            var likes = (await _unitOfWork.Repositry<LikePost, string>().GetAllAsync())
                .Where(p => p.PostId == post.Id).ToList();
            foreach (var like in likes)
                _unitOfWork.Repositry<LikePost, string>().Delete(like);

            // حذف الكومنتات المرتبطة بالبوست
            var comments = (await _unitOfWork.Repositry<Comment, string>().GetAllAsync())
                .Where(p => p.PostId == post.Id).ToList();
            foreach (var comment in comments)
                _unitOfWork.Repositry<Comment, string>().Delete(comment);

            // أخيرًا احذف البوست نفسه
            _unitOfWork.Repositry<Post, string>().Delete(post);

            if (await _unitOfWork.CompleteAsync() <= 0)
                response =  new Response
                {
                    Status = "Failed",
                    Message = "حصل خطأ اثناء مسح منشورك"
                };

            
            else
            {
                
                response =  new Response
                {
                    Status = "Success",
                    Message = "تم مسح منشورك بنجاح"
                };
            }

            return new ResponseForUpdateAndDeleteDto
            {
                Response = response,
                imagePath = post.imagePath
            };
                
        } 

        public async Task<IEnumerable<PostDto>> GetAllPosts(PostSpecificationParameters specsParameters , string UserId)
        {
            var spec = new PostSpecification(specsParameters);
            var posts = await _unitOfWork.Repositry<Post , string>().GetAllWithSpecAsync(spec);
            var MappedPosts = new List<PostDto>();
            foreach(var post in posts)
            {
                var UserLikes = await _unitOfWork.Repositry<LikePost, string>().GetAllAsync();
                var user = UserLikes.FirstOrDefault(x => x.UserId == UserId && x.PostId == post.Id);
                int LikesLength = UserLikes.Count(x => x.PostId == post.Id);
                bool IsLike = false;
                var Comments = await _commentService.GetCommentsAsync(new CommentSpecificationParameter { PostId = post.Id }, UserId);
                int CommentsLength = Comments.Count(); 
                if (user is not null)
                    IsLike = true;
                MappedPosts.Add(new PostDto
                {
                    Id = post.Id,
                    Content = post.Content,
                    DateTime = post.DateTime,
                    User = new UserPostDto
                    {
                        UserId = post.User.Id,
                        Name = $"{post.User.FirstName} {post.User.LastName}",
                        ImagePath = post.User.ImageUrl ?? ""
                    },
                    IsYouLike = IsLike,
                    NumberOfLikes = LikesLength,
                    NumberOfComments = CommentsLength,
                    imagePath = post.imagePath

                });
            }

            return MappedPosts;
        }

        public async Task<IEnumerable<PostDto>> GetAllPostsForMyFollowings(string UserId)
        {
            var followings = await _followingService.GetFollowings(UserId);
            var followingPosts = new List<PostDto>();
            foreach(var following in followings)
            {
                var posts = await GetAllPosts(new PostSpecificationParameters { UserId = following.UserId } , UserId);
                followingPosts.AddRange(posts);
            }
            
            return followingPosts.OrderByDescending(x => x.DateTime);

        }

        public async Task<PostDto> GetPost(string Id , string UserId  )
        {
            var specs = new PostSpecification(Id);
            var post = await _unitOfWork.Repositry<Post , string>().GetWithSpecAsync(specs);
            if (post is null)
                throw new Exception("No Posts With This Id");

            var UserLikes = await _unitOfWork.Repositry<LikePost, string>().GetAllAsync();
            var user = UserLikes.FirstOrDefault(x => x.UserId == UserId && x.PostId == post.Id);
            int LikesLength = UserLikes.Count(x => x.PostId == post.Id);
            bool IsLike = false;
            var Comments = await _commentService.GetCommentsAsync(new CommentSpecificationParameter { PostId = post.Id } , UserId);
            int CommentsLength = Comments.Count();
            if (user is not null)
                IsLike = true;
            return new PostDto
            {
                Id = post.Id,
                Content = post.Content,
                DateTime = post.DateTime,
                User = new UserPostDto
                {
                    UserId = post.User.Id,
                    Name = $"{post.User.FirstName} {post.User.LastName}",
                    ImagePath = post.User.ImageUrl ?? ""
                },
                IsYouLike = IsLike,
                NumberOfComments = CommentsLength,
                NumberOfLikes = LikesLength , 
                imagePath = post.imagePath
            };
        }

        public async Task<ResponseForUpdateAndDeleteDto> UpdatePost(UpdatePostDto inputPost, string PostId , string UserId)
        {
            string oldImagePath = "";
            var specs = new PostSpecification(PostId);
            Response response = new Response();
            var post = await _unitOfWork.Repositry<Post, string>().GetWithSpecAsync(specs);
            oldImagePath = post.imagePath ?? "";
            if (post is null)
                response =  new Response
                {
                    Status = "Failed",
                    Message = "لا يوجد منشور بهذا المعرف"
                };
            if (UserId != post.UserId)
                response =  new Response
                {
                    Status = "Failed",
                    Message = "لا يمكنك تعديل هذا المنشور"
                };

            post.Content = inputPost.NewContent;
            post.imagePath = inputPost.imagePath;

            
                _unitOfWork.Repositry<Post, string>().Update(post);
                if (await _unitOfWork.CompleteAsync() <= 0)
                    response = new Response
                    {
                        Status = "Failed",
                        Message = "حصل خطأ اثناء تعديل منشورك"
                    };



                response = new Response
                {
                    Status = "Success",
                    Message = "تم تعديل منشورك بنجاح"
                };
          


            return new ResponseForUpdateAndDeleteDto
            {
                Response = response,
                imagePath = oldImagePath
            };
        }


    }
}
