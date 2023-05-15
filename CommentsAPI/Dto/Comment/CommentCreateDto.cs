namespace CommentsAPI.Dto.Comment
{
    public class CommentCreateDto
    {
        public string Text { get; init; }
        public Guid PostId { get; init; }
        public Guid UserId { get; init; }
    }
}
