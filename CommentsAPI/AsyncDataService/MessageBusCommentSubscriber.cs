using CommentsAPI.EventProcessing;
using Infrastructure.AsyncDataServices;

namespace CommentsAPI.AsyncDataService;

public class MessageBusCommentSubscriber : MessageBusSubscriber
{
    private readonly IEventProcessor _eventProcessor;
    
    public MessageBusCommentSubscriber(IEventProcessor eventProcessor)
    {
        _eventProcessor = eventProcessor;
    }

    protected override void ProcessEvent(string message)
    {
        _eventProcessor.ProcessEvent(message);
    }
}