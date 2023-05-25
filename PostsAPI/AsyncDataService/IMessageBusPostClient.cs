using Infrastructure.AsyncDataServices.Dto;

namespace PostsAPI.AsyncDataService;

public interface IMessageBusPostClient
{
    void PublishPostDeleteEvent(PostDeletedPublisherDto dto);
}