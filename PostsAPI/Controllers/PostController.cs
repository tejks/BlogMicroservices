using Core.Entities.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using PostsAPI.AsyncDataServices.Grpc.Client;
using PostsAPI.Dto;

namespace PostsAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IPostRepository _context;
        private readonly IUserRepository _userContext;
        private readonly IGrpcCommentClient _grpc;

        public PostsController(ILogger<PostsController> logger, IPostRepository context, IUserRepository userContext, IGrpcCommentClient grpc)
        {
            _logger = logger;
            _context = context;
            _userContext = userContext;
            _grpc = grpc;
        }

        [HttpGet(Name = "GetPosts")]
        public async Task<IEnumerable<Post>> Get()
        {
            return await _context.GetPostsAsync();
        }

        [HttpPost(Name = "CreatePosts")]
        public async Task<Post> Post(PostCreateDto postDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@test",
                FirstName = "Test",
                LastName = "Test",
                CreatedDate = DateTimeOffset.UtcNow
            };

            var data = _grpc.ReturnAllComments();
            Console.WriteLine(data);

            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Title = postDto.Title,
                Text = data,
                CreatedDate = DateTimeOffset.UtcNow
            };

            _logger.LogInformation("Data");


            await _userContext.CreateUserAsync(user);
            await _context.CreatePostAsync(post);

            return post;
        }
    }
}
