namespace CommentsAPI.EventProcessing;

public interface IEventProcessor
{
    void ProcessEvent(string message);
}