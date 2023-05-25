using System.Text.Json;
using Infrastructure.AsyncDataServices;
using Infrastructure.AsyncDataServices.Dto;

namespace PostsAPI.AsyncDataService;

public class MessageBusPostClient : MessageBusClient, IMessageBusPostClient
{
    public MessageBusPostClient(IConfiguration configuration) : base(configuration)
    {
    }

    public void PublishPostDeleteEvent(PostDeletedPublisherDto dto)
    {
        var message = JsonSerializer.Serialize(dto);

        if (Connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ connection open, sending message...");
            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ connection closed, not sending");
        }
    }
}