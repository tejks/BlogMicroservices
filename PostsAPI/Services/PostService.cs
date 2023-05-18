using System.Linq.Expressions;
using Core.Entities.Models;
using Core.Repositories;
using PostsAPI.Dto.Post;

namespace PostsAPI.Services;

public class PostService: IPostService
{
    private readonly ILogger<PostService> _logger;
    private readonly IPostRepository _postRepository;
    
    public PostService(ILogger<PostService> logger, IPostRepository postRepository)
    {
        _logger = logger;
        _postRepository = postRepository;
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

    public async Task<PostDto> CreateAsync(PostDto entity)
    {
        var post = new Post()
        {
            Id = Guid.NewGuid(),
            Title = entity.Title,
            Text = entity.Text,
            UserId = entity.UserId,
            CreatedDate = DateTimeOffset.UtcNow
        };
        
        await _postRepository.CreateAsync(post);
        var newPost = await _postRepository.GetByIdAsync(post.Id);

        return PostDto.PostToDto(newPost);
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

    public async Task UpdateAsync(PostDto entity)
    {
        var post = new Post()
        {
            Id = entity.Id,
            Title = entity.Title,
            Text = entity.Text,
            UserId = entity.UserId,
            CreatedDate = entity.CreatedDate
        };

        await _postRepository.UpdateAsync(post);
    }


    public async Task UpdateAsync(Guid id, PostCreateDto entity)
    {
        var oldPost = await GetByIdAsync(id);
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