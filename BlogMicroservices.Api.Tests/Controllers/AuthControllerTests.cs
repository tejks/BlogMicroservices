using System;
using System.Threading.Tasks;
using AuthAPI.Controllers;
using AuthAPI.Dto;
using AuthAPI.Models;
using AuthAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BlogMicroservices.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly AuthController _controller;
    private readonly Mock<IUserService> _userServiceMock;

    public AuthControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _accountServiceMock = new Mock<IAccountService>();
        _controller = new AuthController(_userServiceMock.Object, _accountServiceMock.Object);
    }

    #region Register user tests

    [Fact]
    public async Task Register_ValidUserData_ReturnsOk()
    {
        // Arrange
        var userCreateDto = new UserCreateDto
            {FirstName = "John", LastName = "Doe", Email = "test@example.com", Password = "password"};
        var newUser = new UserDto
            {Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "test@example.com"};

        _accountServiceMock.Setup(service => service.Register(userCreateDto)).ReturnsAsync(newUser);

        // Act
        var result = await _controller.Register(userCreateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsAssignableFrom<UserDto>(okResult.Value);

        Assert.Equal(newUser, returnedUser);
    }

    [Fact]
    public async Task Register_ExistingEmail_ReturnsConflict()
    {
        // Arrange
        var userCreateDto = new UserCreateDto
            {FirstName = "John", LastName = "Doe", Email = "test@example.com", Password = "password"};

        _accountServiceMock.Setup(service => service.Register(userCreateDto)).ReturnsAsync((UserDto) null);

        // Act
        var result = await _controller.Register(userCreateDto);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        var errorMessage = conflictResult.Value.GetType().GetProperty("error_message").GetValue(conflictResult.Value) as string;

        Assert.Equal("Email is already in use.", errorMessage);
    }

    #endregion

    #region Login user tests

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var userLoginDto = new UserLoginDto {Email = "test@example.com", Password = "password"};
        var loginResult = new UserAuthResponse {IsAuthenticated = true, Message = "Login successful"};

        _accountServiceMock.Setup(service => service.Login(userLoginDto)).ReturnsAsync(loginResult);

        // Act
        var result = await _controller.Login(userLoginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsAssignableFrom<UserAuthResponse>(okResult.Value);

        Assert.Equal(loginResult, returnedResult);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var userLoginDto = new UserLoginDto {Email = "test@example.com", Password = "invalidpassword"};
        var loginResult = new UserAuthResponse {IsAuthenticated = false, Message = "Invalid credentials"};

        _accountServiceMock.Setup(service => service.Login(userLoginDto)).ReturnsAsync(loginResult);

        // Act
        var result = await _controller.Login(userLoginDto);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var errorMessage = unauthorizedResult.Value.GetType().GetProperty("error_message").GetValue(unauthorizedResult.Value) as string;

        Assert.Equal(loginResult.Message, errorMessage);
    }

    #endregion
}