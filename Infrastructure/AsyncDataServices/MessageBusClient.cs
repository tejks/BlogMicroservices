using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Infrastructure.AsyncDataServices;

public abstract class MessageBusClient
{
    protected readonly IConfiguration Configuration;
    protected readonly IConnection Connection;
    protected readonly IModel Channel;

    protected MessageBusClient(IConfiguration configuration)
    {
        Configuration = configuration;
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://guest:guest@rabbitmq:5672/")
        };
        
        try
        {
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();

            Channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            Console.WriteLine("--> Connected to MessageBus");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
        }
    }

    protected void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        Channel.BasicPublish(exchange: "trigger",
            routingKey: "",
            basicProperties: null,
            body: body);
        
        Console.WriteLine($"--> We have sent {message}");
    }

    public void Dispose()
    {
        Console.WriteLine("MessageBus Disposed");
        if (!Channel.IsOpen) return;
        Channel.Close();
        Connection.Close();
    }
}