using System.Security.Claims;
using CommentsAPI.Dto.Comment;
using CommentsAPI.Services;
using Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommentsAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IEnumerable<CommentDto>> GetAllComments()
        {
            return await _commentService.GetAllAsync();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CommentDto>> GetComment(Guid id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            if (comment == null) return NotFound();
            return comment;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CommentDto>> PostComment(CommentCreateDto commentCreateDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var newComment = await _commentService.CreateAsync(userId, commentCreateDto);

            return CreatedAtAction(nameof(GetComment), new {id = newComment.Id}, newComment);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> PutComment(Guid id, CommentUpdateDto commentUpdateDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var comment = await _commentService.GetByIdAsync(id);

            if (comment == null) return NotFound();
            if (!comment.UserId.Equals(userId))
                return Unauthorized(new {error_message = "The comment does not belong to this user"});
            
            await _commentService.UpdateAsync(id, commentUpdateDto);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var comment = await _commentService.GetByIdAsync(id);

            if (comment == null) return NotFound();
            if (!comment.UserId.Equals(userId))
                return Unauthorized(new {error_message = "The comment does not belong to this user"});

            await _commentService.DeleteAsync(id);

            return NoContent();
        }
    }
}
