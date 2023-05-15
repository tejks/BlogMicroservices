namespace PostsAPI.Dto.Post
{
    public class PostDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string Text { get; init; }
        public Guid UserId { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}