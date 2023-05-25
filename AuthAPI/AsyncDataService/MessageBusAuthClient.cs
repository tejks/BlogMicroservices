using System.Text.Json;
using Infrastructure.AsyncDataServices;
using Infrastructure.AsyncDataServices.Dto;

namespace AuthAPI.AsyncDataService;

public class MessageBusAuthClient : MessageBusClient, IMessageBusAuthClient
{
    public MessageBusAuthClient(IConfiguration configuration) : base(configuration)
    {
    }

    public void PublishUserDeleteEvent(UserDeletedPublisherDto dto)
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