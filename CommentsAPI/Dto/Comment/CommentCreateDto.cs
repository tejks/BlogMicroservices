namespace CommentsAPI.Dto.Comment
{
    public class CommentCreateDto
    {
        public Guid PostId { get; init; }
        public string Text { get; init; }
    }
}
