using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CommentsAPI.Controllers;
using CommentsAPI.Dto.Comment;
using CommentsAPI.Services;
using Core.Entities.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BlogMicroservices.Api.Tests.Controllers;

public class CommentControllerTests
{
    private readonly Mock<ICommentService> _commentServiceMock;

    private readonly CommentsController _controller;
    private readonly Mock<IPostRepository> _postContextMock;

    public CommentControllerTests()
    {
        _commentServiceMock = new Mock<ICommentService>();
        _postContextMock = new Mock<IPostRepository>();
        _controller = new CommentsController(_commentServiceMock.Object, _postContextMock.Object);
    }

    #region GetAllComments tests

    [Fact]
    public async Task GetAllComments_ReturnsOkWithAllComments()
    {
        // Arrange
        var expectedComments = new List<CommentDto>
        {
            new() { Id = Guid.NewGuid(), Text = "Comment 1", CreatedDate = DateTimeOffset.Now, PostId = Guid.NewGuid(), UserId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Text = "Comment 2", CreatedDate = DateTimeOffset.Now, PostId = Guid.NewGuid(), UserId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Text = "Comment 3", CreatedDate = DateTimeOffset.Now, PostId = Guid.NewGuid(), UserId = Guid.NewGuid() }
        };

        _commentServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(expectedComments);

        // Act
        var result = await _controller.GetAllComments();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<CommentDto>>>(result);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var actualComments = Assert.IsAssignableFrom<IEnumerable<CommentDto>>(okObjectResult.Value);

        Assert.Equal(expectedComments.Count, actualComments.Count());
        Assert.Equal(expectedComments, actualComments);
    }

    #endregion

    #region PostCommeent tests

    [Fact]
    public async Task PostComment_ValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var commentCreateDto = new CommentCreateDto {Text = "New Comment", PostId = postId};

        var existingPost = new Post
        {
            Id = postId, Title = "Test Post", Text = "Test Post Content", UserId = Guid.NewGuid(),
            CreatedDate = DateTimeOffset.Now
        };
        
        _postContextMock.Setup(context => context.GetByIdAsync(postId)).ReturnsAsync(existingPost);

        var userId = Guid.NewGuid();
        var newComment = new CommentDto
        {
            Id = Guid.NewGuid(), Text = "New Comment", CreatedDate = DateTimeOffset.Now, PostId = postId,
            UserId = userId
        };
        
        _commentServiceMock.Setup(service => service.CreateAsync(userId, commentCreateDto)).ReturnsAsync(newComment);

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
        var result = await _controller.PostComment(commentCreateDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<CommentDto>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var returnedComment = Assert.IsAssignableFrom<CommentDto>(createdAtActionResult.Value);

        Assert.Equal(newComment.Id, returnedComment.Id);
        Assert.Equal(newComment, returnedComment);
    }

    #endregion

    #region GetById tests

    [Fact]
    public async Task GetComment_ValidId_ReturnsOkWithComment()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var expectedComment = new CommentDto
        {
            Id = commentId, Text = "Test Comment", CreatedDate = DateTimeOffset.Now, PostId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        _commentServiceMock.Setup(service => service.GetByIdAsync(commentId)).ReturnsAsync(expectedComment);

        // Act
        var result = await _controller.GetComment(commentId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<CommentDto>>(result);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var actualComment = Assert.IsAssignableFrom<CommentDto>(okObjectResult.Value);

        Assert.Equal(commentId, actualComment.Id);
        Assert.Equal(expectedComment, actualComment);
    }

    [Fact]
    public async Task GetComment_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingCommentId = Guid.NewGuid();
        _commentServiceMock.Setup(service => service.GetByIdAsync(nonExistingCommentId)).ReturnsAsync((CommentDto) null);

        // Act
        var result = await _controller.GetComment(nonExistingCommentId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<CommentDto>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    #endregion

    #region PutComment tests

    [Fact]
    public async Task PutComment_ValidId_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var commentId = Guid.NewGuid();
        var commentUpdateDto = new CommentUpdateDto {Text = "Updated Comment"};

        var existingComment = new CommentDto
        {
            Id = commentId, Text = "Old Comment", CreatedDate = DateTimeOffset.Now, PostId = Guid.NewGuid(),
            UserId = userId
        };
        
        _commentServiceMock.Setup(service => service.GetByIdAsync(commentId)).ReturnsAsync(existingComment);
        _commentServiceMock.Setup(service => service.UpdateAsync(commentId, commentUpdateDto)).Returns(Task.CompletedTask);

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
        var result = await _controller.PutComment(commentId, commentUpdateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PutComment_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistingCommentId = Guid.NewGuid();
        var commentUpdateDto = new CommentUpdateDto {Text = "Updated Comment"};

        _commentServiceMock.Setup(service => service.GetByIdAsync(nonExistingCommentId)).ReturnsAsync((CommentDto) null);

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
        var result = await _controller.PutComment(nonExistingCommentId, commentUpdateDto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion


    #region DeleteComment tests

    [Fact]
    public async Task DeleteComment_ValidId_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var commentId = Guid.NewGuid();

        var existingComment = new CommentDto
        {
            Id = commentId, Text = "Test Comment", CreatedDate = DateTimeOffset.Now, PostId = Guid.NewGuid(),
            UserId = userId
        };

        _commentServiceMock.Setup(service => service.GetByIdAsync(commentId)).ReturnsAsync(existingComment);
        _commentServiceMock.Setup(service => service.DeleteAsync(commentId)).Returns(Task.CompletedTask);

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
        var result = await _controller.DeleteComment(commentId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteComment_InValidId_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistingCommentId = Guid.NewGuid();

        _commentServiceMock.Setup(service => service.GetByIdAsync(nonExistingCommentId)).ReturnsAsync((CommentDto) null);

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
        var result = await _controller.DeleteComment(nonExistingCommentId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion
}