using Infrastructure.AsyncDataServices;
using PostsAPI.EventProcessing;

namespace PostsAPI.AsyncDataService;

public class MessageBusPostSubscriber : MessageBusSubscriber
{
    private readonly IEventProcessor _eventProcessor;
    
    public MessageBusPostSubscriber(IEventProcessor eventProcessor)
    {
        _eventProcessor = eventProcessor;
    }

    protected override void ProcessEvent(string message)
    {
        _eventProcessor.ProcessEvent(message);
    }
}