﻿using System.Text.Json;
using Core.Repositories;
using Infrastructure.AsyncDataServices.Dto;

namespace CommentsAPI.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly ICommentRepository _commentRepository;
    
    public EventProcessor(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public void ProcessEvent(string notificationMessage)
    {
        var eventType = DetermineEvent(notificationMessage);

        switch (eventType)
        {
            case EventType.PostDeleted:
                PostDeleteEvent(notificationMessage);
                break;
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
            case "Post_Deleted":
                Console.WriteLine("--> Post Delete Event Detected");
                return EventType.PostDeleted;
            case "User_Deleted":
                Console.WriteLine("--> User Delete Event Detected");
                return EventType.UserDeleted;
            default:
                Console.WriteLine("--> Could not determine the event type");
                return EventType.Undetermined;
        }
    }

    private void PostDeleteEvent(string postPublishedMessage)
    {
        var postPublishedDto = JsonSerializer.Deserialize<PostDeletedPublisherDto>(postPublishedMessage);

        if (postPublishedDto is null) return;
        
        var commentsList = _commentRepository.GetCommentsByPostIdSync(postPublishedDto.Id);

        foreach (var comment in commentsList)
        {
            _commentRepository.DeleteAsync(comment.Id);
        }
    }
    
    private void UserDeleteEvent(string userPublishedMessage)
    {
        var userPublishedDto = JsonSerializer.Deserialize<UserDeletedPublisherDto>(userPublishedMessage);

        if (userPublishedDto is null) return;
        
        var commentsList = _commentRepository.GetCommentsByUserIdSync(userPublishedDto.userId);

        foreach (var comment in commentsList)
        {
            _commentRepository.DeleteAsync(comment.Id);
        }
    }
}

internal enum EventType
{
    PostDeleted,
    UserDeleted,
    Undetermined
}