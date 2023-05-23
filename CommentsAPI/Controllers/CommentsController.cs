using System.Security.Claims;
using CommentsAPI.Dto.Comment;
using Core.Entities.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommentsAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ILogger<CommentsController> _logger;
        private readonly ICommentRepository _commentContext;
        private readonly IPostRepository _postContext;

        public CommentsController(ILogger<CommentsController> logger, ICommentRepository commentContext, IPostRepository postContext)
        {
            _logger = logger;
            _commentContext = commentContext;
            _postContext = postContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments()
        {
            return (await _commentContext.GetCommentsAsync()).Select(x => ItemToDTO(x)).ToList();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CommentDto>> GetComment(Guid id)
        {
            var item = await _commentContext.GetCommentAsync(id);
            if (item == null) return NotFound();
            return ItemToDTO(item);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CommentDto>> Post(CommentCreateDto commentDto)
        {
            var postItem = await _postContext.GetByIdAsync(commentDto.PostId);
            if (postItem == null) return NotFound("Post not exists");
            
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var post = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PostId = postItem.Id,
                Text = commentDto.Text,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _commentContext.CreateCommentAsync(post);

            return ItemToDTO(post);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> PutComment(Guid id, CommentUpdateDto commentDTO)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var comment = await _commentContext.GetCommentAsync(id);
            
            if (comment == null) return NotFound();
            if (!comment.UserId.Equals(userId)) return Unauthorized(new { error_message = "The comment does not belong to this user" });

            var newComment = new Comment
            {
                Id = comment.Id,
                Text = commentDTO.Text,
                PostId = comment.PostId,
                UserId = comment.UserId,
                CreatedDate = comment.CreatedDate,
            };
            
            await _commentContext.UpdateCommentAsync(newComment);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var comment = await _commentContext.GetCommentAsync(id);
            
            if (comment == null) return NotFound();
            if (!comment.UserId.Equals(userId)) return Unauthorized(new { error_message = "The comment does not belong to this user" });

            await _commentContext.DeleteCommentAsync(id);

            return NoContent();
        }

        private static CommentDto ItemToDTO(Comment commentItem) =>
           new CommentDto
           {
               Id = commentItem.Id,
               Text = commentItem.Text,
               CreatedDate = commentItem.CreatedDate,
               PostId = commentItem.PostId,
               UserId = commentItem.UserId
           };
        }
}
