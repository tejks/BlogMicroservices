using Core.Entities.Models;

namespace CommentsAPI.Dto.Comment
{
    public class CommentDto
    {
        public Guid Id { get; init; }
        public string Text { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
        public Guid PostId { get; init; }
        public Guid UserId { get; init; }
    }
}
