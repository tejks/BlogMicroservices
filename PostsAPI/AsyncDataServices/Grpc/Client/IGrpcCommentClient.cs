using Core.Entities.Models;

namespace PostsAPI.AsyncDataServices.Grpc.Client
{
    public interface IGrpcCommentClient
    {
        string ReturnAllComments();
    }
}
