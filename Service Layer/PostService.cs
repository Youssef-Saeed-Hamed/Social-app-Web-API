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
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILikeService _likeService;
        private readonly ICommentService _commentService;
        private readonly IFollowingService _followingService;
        public PostService(IUnitOfWork unitOfWork, ILikeService likeService, ICommentService commentService, IFollowingService followingService)
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
            };

            await _unitOfWork.Repositry<Post, string>().InsertAsync(post);
            if (await _unitOfWork.CompleteAsync() <= 0)
                return new Response
                {
                    Status = "Failed",
                    Message = "Your Post Not Added Successfully"
                };

            return new Response
            {
                Status = "Success",
                Message = "Your Post Added Successfully"
            };
        }

        public async Task<Response> DeletePost(DeletePostDto deletePost, string UserId)
        {
            var specs = new PostSpecification(deletePost.PostId);
            var post = await _unitOfWork.Repositry<Post, string>().GetWithSpecAsync(specs);
            if (post is null)
                return new Response
                {
                    Status = "Failed",
                    Message = "No Posts With This Id"
                };

            if (UserId != post.UserId)
                return new Response
                {
                    Status = "Failed",
                    Message = "You Can't Delete This Post Because This Is Not Your Post"
                };

            _unitOfWork.Repositry<Post , string>().Delete(post);
            if (await _unitOfWork.CompleteAsync() > 0)
                return new Response
                {
                    Status = "Failed",
                    Message = "Your Post Not Deleted Successfully"
                };

           
            else
                return new Response
                {
                    Status = "Success",
                    Message = "Your Post Deleted Successfully"
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
                        ImagePath = string.IsNullOrWhiteSpace(post.User.ImageUrl) ? "" :
                         Path.Combine(Directory.GetCurrentDirectory(), @"Files/Images", post.User.ImageUrl)
                    },
                    IsYouLike = IsLike,
                    NumberOfLikes = LikesLength,
                    NumberOfComments = CommentsLength

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
                    ImagePath = string.IsNullOrWhiteSpace(post.User.ImageUrl) ? "" :
                         Path.Combine(Directory.GetCurrentDirectory(), @"Files/Images", post.User.ImageUrl, UserId)
                },
                IsYouLike = IsLike,
                NumberOfComments = CommentsLength,
                NumberOfLikes = LikesLength
            };
        }

        public async Task<Response> UpdatePost(UpdatePostDto inputPost, string PostId , string UserId)
        {

            var specs = new PostSpecification(PostId);
            var post = await _unitOfWork.Repositry<Post, string>().GetWithSpecAsync(specs); if (post is null)
                return new Response
                {
                    Status = "Failed",
                    Message = "No Posts With This Id"
                };
            if (UserId != post.UserId)
                return new Response
                {
                    Status = "Failed",
                    Message = "You Can't Update This Post Because This Is Not Your Post"
                };

            post.Content = inputPost.NewContent;

            _unitOfWork.Repositry<Post, string>().Update(post);
            if (await _unitOfWork.CompleteAsync() <= 0)
                return new Response
                {
                    Status = "Failed",
                    Message = "Your Post Not Updated Successfully"
                };



            return new Response
            {
                Status = "Success",
                Message = "Your Post Updated Successfully"
            };

        }


    }
}
