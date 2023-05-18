namespace AuthAPI.Models;

public class AuthTokenResponse
{
    public Guid UserId { get; set; }
    public string Token { get; set; }

}