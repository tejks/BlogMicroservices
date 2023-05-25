namespace PostsAPI.EventProcessing;

public interface IEventProcessor
{
    void ProcessEvent(string message);
}