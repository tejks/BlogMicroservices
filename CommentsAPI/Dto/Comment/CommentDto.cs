using Core.Entities;
using Core.Entities.Models;

namespace CommentsAPI.Dto.Comment
{
    public class CommentDto : IEntityBase
    {
        public Guid Id { get; init; }
        public string Text { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
        public Guid PostId { get; init; }
        public Guid UserId { get; init; }

        public static CommentDto CommentToDto(Core.Entities.Models.Comment comment)
        {
            return new CommentDto()
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedDate = comment.CreatedDate,
                PostId = comment.PostId,
                UserId = comment.UserId,
            };
        }
    }
}
