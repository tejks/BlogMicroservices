using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsAPI.Dto.Post;
using PostsAPI.Services;
using PostsAPI.SyncDataServices.Grpc.Client;

namespace PostsAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IGrpcCommentClient _grpc;

        public PostsController(IPostService postService, IGrpcCommentClient grpc)
        {
            _postService = postService;
            _grpc = grpc;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetAllPosts()
        {
            return Ok(await _postService.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PostDto>> GetPost(Guid id)
        {
            var post = await _postService.GetByIdAsync(id);
            if (post is null) return NotFound();
            
            return Ok(post);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostDto>> PostPost(PostCreateDto postCreateDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var newPost = await _postService.CreateAsync(userId, postCreateDto);

            return CreatedAtAction(nameof(GetPost), new {id = newPost.Id}, newPost);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> PutPost(Guid id, PostUpdateDto postUpdateDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var post = await _postService.GetByIdAsync(id);

            if (post is null) return NotFound();
            if (!post.UserId.Equals(userId)) return Unauthorized(new { error_message = "The post does not belong to this user" });

            await _postService.UpdateAsync(id, postUpdateDto);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var post = await _postService.GetByIdAsync(id);

            if (post is null) return NotFound();
            if (!post.UserId.Equals(userId)) return Unauthorized(new { error_message = "The post does not belong to this user" });

            await _postService.DeleteAsync(id);

            return NoContent();
        }
    }
}
