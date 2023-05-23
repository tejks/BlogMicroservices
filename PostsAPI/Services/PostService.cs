using System.Linq.Expressions;
using Core.Entities.Models;
using Core.Repositories;
using PostsAPI.Dto.Post;

namespace PostsAPI.Services;

public class PostService: IPostService
{
    private readonly IPostRepository _postRepository;
    
    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<IEnumerable<PostDto>> GetAllByUserIdAsync(Guid userId)
    {
        var posts = await _postRepository.GetAllAsync();
        var userPosts = posts.Where(x => x.UserId.Equals(userId)).Select(PostDto.PostToDto).OrderByDescending(x => x.CreatedDate).ToList();

        return userPosts;
    }

    public async Task<PostDto> GetUserLastPostAsync(Guid userId)
    {
        // Posts are already ordered descending
        var posts = await _postRepository.GetAllAsync();
        var lastPost = posts.Where(x => x.UserId.Equals(userId)).Select(PostDto.PostToDto).OrderByDescending(x => x.CreatedDate).Take(1).FirstOrDefault();
        
        return lastPost;
    }

    public async Task<IEnumerable<PostDto>> GetAllAsync()
    {
        var posts = await _postRepository.GetAllAsync();
        var postsDto = posts.Select(PostDto.PostToDto);

        return postsDto;
    }

    public async Task<PostDto> GetByIdAsync(Guid id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        return post is null ? null : PostDto.PostToDto(post);
    }

    public async Task<PostDto> CreateAsync(Guid userId, PostCreateDto entity)
    {
        var post = new Post()
        {
            Id = Guid.NewGuid(),
            Title = entity.Title,
            Text = entity.Text,
            UserId = userId,
            CreatedDate = DateTimeOffset.UtcNow
        };
        
        await _postRepository.CreateAsync(post);
        var newPost = await _postRepository.GetByIdAsync(post.Id);

        return PostDto.PostToDto(newPost);
    }
    
    public async Task UpdateAsync(Guid postId, PostUpdateDto entity)
    {
        var oldPost = await GetByIdAsync(postId);
        var post = new Post()
        {
            Id = oldPost.Id,
            Title = entity.Title,
            Text = entity.Text,
            UserId = oldPost.UserId,
            CreatedDate = oldPost.CreatedDate
        };

        await _postRepository.UpdateAsync(post);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _postRepository.DeleteAsync(id);
    }
}