using Core.Entities.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using PostsAPI.AsyncDataServices.Grpc.Client;
using PostsAPI.Dto.Post;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts()
        {
            return (await _context.GetPostsAsync()).Select(x => ItemToDTO(x)).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(Guid id)
        {
            var item = await _context.GetPostAsync(id);
            if (item == null) return NotFound();
            return ItemToDTO(item);
        }

        [HttpPost]
        public async Task<ActionResult<PostDto>> Post(PostCreateDto postDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@test",
                FirstName = "Test",
                LastName = "Test",
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _userContext.CreateUserAsync(user);

            var userItem = await _userContext.GetUserAsync(user.Id);

            if(userItem == null) return NotFound("User not exists");

            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Title = postDto.Title,
                Text = "Data",
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _context.CreatePostAsync(post);

            return ItemToDTO(post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(Guid id, PostUpdateDto postDTO)
        {
            var postItem = await _context.GetPostAsync(id);
            if (postItem == null)
            {
                return NotFound();
            }

            var newPost = new Post
            {
                Id = id,
                Title = postDTO.Title,
                Text = postDTO.Text,
                CreatedDate = DateTimeOffset.UtcNow
            };

            try
            {
                await _context.UpdatePostAsync(newPost);
            }
            catch
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(Guid id)
        {
            var postItem = await _context.GetPostAsync(id);
            if (postItem == null)
            {
                return NotFound();
            }

            await _context.DeletePostAsync(id);

            return NoContent();
        }

        private static PostDto ItemToDTO(Post postItem) =>
           new PostDto
           {
               Id = postItem.Id,
               Title= postItem.Title,
               Text = postItem.Text,
               CreatedDate = postItem.CreatedDate
           };
        }
}
