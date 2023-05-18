namespace AuthAPI.Configuration;

public class JwtSettings
{
    private const string Section = "JwtSettings";

    private readonly IConfiguration _configuration;
    
    public string? Secret => _configuration.GetSection(Section).GetSection("Secret").Value;
    public string? Issuer => _configuration.GetSection(Section).GetSection("Issuer").Value;
    public string? Audience => _configuration.GetSection(Section).GetSection("Audience").Value;
    
    
    public JwtSettings(IConfiguration configuration)
    {
        _configuration = configuration;
    }
}