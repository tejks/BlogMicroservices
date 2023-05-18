using Core.Entities.Models;
using Core.Repositories;
using PostsAPI.Dto.Post;

namespace PostsAPI.Services;

public interface IPostService: IGenericRepository<PostDto>
{
    Task<PostDto> CreateAsync(Guid userId, PostCreateDto entity);
    Task UpdateAsync(Guid id, PostCreateDto entity);
}