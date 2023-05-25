namespace Infrastructure.AsyncDataServices.Dto;

public class UserDeletedPublisherDto
{
    public Guid userId { get; set; }
    public string Event { get; set; }
}