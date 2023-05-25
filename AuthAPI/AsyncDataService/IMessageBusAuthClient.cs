using Infrastructure.AsyncDataServices.Dto;

namespace AuthAPI.AsyncDataService;

public interface IMessageBusAuthClient
{
    void PublishUserDeleteEvent(UserDeletedPublisherDto dto);
}