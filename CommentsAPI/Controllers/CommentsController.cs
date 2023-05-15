using CommentsAPI.Dto.Comment;
using Core.Entities.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CommentsAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ILogger<CommentsController> _logger;
        private readonly ICommentRepository _commentContext;
        private readonly IUserRepository _userContext;
        private readonly IPostRepository _postContext;

        public CommentsController(ILogger<CommentsController> logger, ICommentRepository commentContext, IUserRepository userContext, IPostRepository postContext)
        {
            _logger = logger;
            _commentContext = commentContext;
            _userContext = userContext;
            _postContext = postContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments()
        {
            return (await _commentContext.GetCommentsAsync()).Select(x => ItemToDTO(x)).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(Guid id)
        {
            var item = await _commentContext.GetCommentAsync(id);
            if (item == null) return NotFound();
            return ItemToDTO(item);
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> Post(CommentCreateDto commentDto)
        {
            var userItem = await _userContext.GetUserAsync(commentDto.UserId);

            if (userItem == null) return NotFound("User not exists");

            var postItem = await _postContext.GetPostAsync(commentDto.PostId);

            if (postItem == null) return NotFound("Post not exists");

            var post = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = userItem.Id,
                PostId = postItem.Id,
                Text = commentDto.Text,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _commentContext.CreateCommentAsync(post);

            return ItemToDTO(post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(Guid id, CommentUpdateDto commentDTO)
        {
            var commentItem = await _commentContext.GetCommentAsync(id);
            if (commentItem == null)
            {
                return NotFound();
            }

            var newComment = new Comment
            {
                Id = commentItem.Id,
                Text = commentDTO.Text,
                PostId = commentItem.PostId,
                UserId = commentItem.UserId,
                CreatedDate = commentItem.CreatedDate,
            };

            try
            {
                await _commentContext.UpdateCommentAsync(newComment);
            }
            catch
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var commentItem = await _commentContext.GetCommentAsync(id);
            if (commentItem == null)
            {
                return NotFound();
            }

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
