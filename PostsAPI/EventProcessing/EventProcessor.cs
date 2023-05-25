using System.Text.Json;
using Core.Repositories;
using Infrastructure.AsyncDataServices.Dto;

namespace PostsAPI.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IPostRepository _postRepository;
    
    public EventProcessor(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public void ProcessEvent(string notificationMessage)
    {
        var eventType = DetermineEvent(notificationMessage);

        switch (eventType)
        {
            case EventType.UserDeleted:
                UserDeleteEvent(notificationMessage);
                break;
            case EventType.Undetermined:
                break;
            default:
                break;
        }
    }
    
    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEvent>(notificationMessage);

        if (eventType is null) return EventType.Undetermined;
        
        switch(eventType.Event)
        {
            case "User_Deleted":
                Console.WriteLine("--> User Delete Event Detected");
                return EventType.UserDeleted;
            default:
                Console.WriteLine("--> Could not determine the event type");
                return EventType.Undetermined;
        }
    }

    private void UserDeleteEvent(string userPublishedMessage)
    {
        var userPublishedDto = JsonSerializer.Deserialize<UserDeletedPublisherDto>(userPublishedMessage);

        if (userPublishedDto is null) return;
        
        var commentsList = _postRepository.GetCommentsByUserIdSync(userPublishedDto.userId);

        foreach (var comment in commentsList)
        {
            _postRepository.DeleteAsync(comment.Id);
        }
    }
}

internal enum EventType
{
    UserDeleted,
    Undetermined
}