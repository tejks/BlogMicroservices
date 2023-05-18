namespace AuthAPI.Dto;

public class UserCreateDto
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string Password { get; set; }
}