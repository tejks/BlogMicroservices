using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsAPI.Dto.Post;
using PostsAPI.Services;
using PostsAPI.SyncDataServices.Grpc.Client;

namespace PostsAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IPostService _postService;
        private readonly IGrpcCommentClient _grpc;

        public PostsController(ILogger<PostsController> logger, IPostService postService, IGrpcCommentClient grpc)
        {
            _logger = logger;
            _postService = postService;
            _grpc = grpc;
        }

        [HttpGet]
        public async Task<IEnumerable<PostDto>> GetPosts()
        {
            var posts = await _postService.GetAllAsync();
            return posts;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(Guid id)
        {
            var post = await _postService.GetByIdAsync(id);

            if (post is null)
            {
                return NotFound();
            }

            return post;
        }

        [HttpPost]
        public async Task<ActionResult<PostDto>> Post(PostCreateDto postCreateDto)
        {
            var userId = Guid.Empty;
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (userIdStr is not null)
            {
                userId = Guid.Parse(userIdStr);
            }
            var newPost = await _postService.CreateAsync(userId, postCreateDto);

            return CreatedAtAction(nameof(GetPost), new {id = newPost.Id}, newPost);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(Guid id, PostCreateDto postUpdateDto)
        {
            var post = await _postService.GetByIdAsync(id);

            if (post is null)
            {
                return NotFound();
            }

            await _postService.UpdateAsync(id, postUpdateDto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(Guid id)
        {
            var post = await _postService.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            await _postService.DeleteAsync(id);

            return NoContent();
        }
    }
}
