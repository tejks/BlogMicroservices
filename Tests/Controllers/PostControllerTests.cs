using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostsAPI.Controllers;
using PostsAPI.Dto.Post;
using PostsAPI.Services;
using PostsAPI.SyncDataServices.Grpc.Client;
using Xunit;
using Moq;

namespace Tests.Controllers;

public class PostControllerTests
{
    private readonly PostsController _controller;
    private readonly Mock<IGrpcCommentClient> _grpcClientMock;
    private readonly Mock<IPostService> _postServiceMock;

    public PostControllerTests()
    {
        _postServiceMock = new Mock<IPostService>();
        _grpcClientMock = new Mock<IGrpcCommentClient>();
        _controller = new PostsController(_postServiceMock.Object, _grpcClientMock.Object);
    }

    #region GetAll tests

    [Fact]
    public async Task GetAllPosts_ReturnsOkWithAllPosts()
    {
        // Arrange
        var expectedPosts = new List<PostDto>
        {
            new() { Id = Guid.NewGuid(), Title = "Post 1", Text = "Text 1", UserId = Guid.NewGuid(), CreatedDate = DateTimeOffset.Now },
            new() { Id = Guid.NewGuid(), Title = "Post 2", Text = "Text 2", UserId = Guid.NewGuid(), CreatedDate = DateTimeOffset.Now },
            new() { Id = Guid.NewGuid(), Title = "Post 3", Text = "Content 3", UserId = Guid.NewGuid(), CreatedDate = DateTimeOffset.Now }
        };

        _postServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(expectedPosts);

        // Act
        var result = await _controller.GetAllPosts();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<PostDto>>>(result);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var actualPosts = Assert.IsAssignableFrom<IEnumerable<PostDto>>(okObjectResult.Value);

        Assert.Equal(expectedPosts.Count, actualPosts.Count());
        Assert.Equal(expectedPosts, actualPosts);
        // Add more assertions as per your requirements
    }

    #endregion

    #region PostPost tests :)

    [Fact]
    public async Task PostPost_ValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var postCreateDto = new PostCreateDto {Title = "New Post", Text = "Post content"};
        var newPost = new PostDto { Id = Guid.NewGuid(), Title = "New Post", Text = "Post content", UserId = userId, CreatedDate = DateTimeOffset.Now };

        _postServiceMock.Setup(service => service.CreateAsync(userId, postCreateDto)).ReturnsAsync(newPost);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, "TestAuthentication"))
            }
        };

        // Act
        var result = await _controller.PostPost(postCreateDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<PostDto>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var createdPost = Assert.IsAssignableFrom<PostDto>(createdAtActionResult.Value);

        Assert.Equal(newPost.Id, createdPost.Id);
        Assert.Equal(newPost.Title, createdPost.Title);
        Assert.Equal(newPost.Text, createdPost.Text);
        Assert.Equal(newPost.UserId, createdPost.UserId);
        Assert.Equal(newPost.CreatedDate, createdPost.CreatedDate);
    }

    #endregion

    #region GetById tests

    [Fact]
    public async Task GetPost_ValidId_ReturnsOkWithPost()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var expectedPost = new PostDto { Id = postId, Title = "Test Post", Text = "Text 1", UserId = Guid.NewGuid(), CreatedDate = DateTimeOffset.Now };

        _postServiceMock.Setup(service => service.GetByIdAsync(postId)).ReturnsAsync(expectedPost);

        // Act
        var result = await _controller.GetPost(postId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<PostDto>>(result);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var actualPost = Assert.IsAssignableFrom<PostDto>(okObjectResult.Value);

        Assert.Equal(expectedPost.Id, actualPost.Id);
        Assert.Equal(expectedPost.Title, actualPost.Title);
        // Add more assertions as per your requirements
    }

    [Fact]
    public async Task GetPost_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidPostId = Guid.NewGuid();

        _postServiceMock.Setup(service => service.GetByIdAsync(invalidPostId));

        // Act
        var result = await _controller.GetPost(invalidPostId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<PostDto>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    #endregion

    #region PutPost tests

    [Fact]
    public async Task PutPost_ValidId_ReturnsNoContent()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var postUpdateDto = new PostUpdateDto {Title = "Updated Post", Text = "Updated content"};

        var post = new PostDto { Id = postId, Title = "Old Post", Text = "Old content", UserId = userId, CreatedDate = DateTimeOffset.Now };

        _postServiceMock.Setup(service => service.GetByIdAsync(postId)).ReturnsAsync(post);
        _postServiceMock.Setup(service => service.UpdateAsync(postId, postUpdateDto)).Returns(Task.CompletedTask);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, "TestAuthentication"))
            }
        };

        // Act
        var result = await _controller.PutPost(postId, postUpdateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PutPost_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidPostId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var postUpdateDto = new PostUpdateDto {Title = "Updated Post", Text = "Updated content"};

        _postServiceMock.Setup(service => service.GetByIdAsync(invalidPostId)).ReturnsAsync(null as PostDto);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, "TestAuthentication"))
            }
        };

        // Act
        var result = await _controller.PutPost(invalidPostId, postUpdateDto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region DeletPost tests

    [Fact]
    public async Task DeletePost_ValidId_ReturnsNoContent()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var post = new PostDto
        {
            Id = postId, Title = "Post to delete", Text = "Content to delete", UserId = userId,
            CreatedDate = DateTimeOffset.Now
        };

        _postServiceMock.Setup(service => service.GetByIdAsync(postId)).ReturnsAsync(post);
        _postServiceMock.Setup(service => service.DeleteAsync(postId)).Returns(Task.CompletedTask);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, "TestAuthentication"))
            }
        };

        // Act
        var result = await _controller.DeletePost(postId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeletePost_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidPostId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _postServiceMock.Setup(service => service.GetByIdAsync(invalidPostId)).ReturnsAsync(null as PostDto);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, "TestAuthentication"))
            }
        };

        // Act
        var result = await _controller.DeletePost(invalidPostId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion
}