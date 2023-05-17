using Core.Entities;

namespace PostsAPI.Dto.Post
{
    public class PostDto : PostCreateDto, IEntityBase
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
        
        public static PostDto PostToDto(Core.Entities.Models.Post post)
        {
            return new PostDto()
            {
                Id = post.Id,
                Title = post.Title,
                Text = post.Text,
                UserId = post.UserId,
                CreatedDate = post.CreatedDate
            };
        }
    }
}