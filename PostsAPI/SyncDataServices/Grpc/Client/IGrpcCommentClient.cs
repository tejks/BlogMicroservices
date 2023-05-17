using Core.Entities.Models;

namespace PostsAPI.SyncDataServices.Grpc.Client
{
    public interface IGrpcCommentClient
    {
        string ReturnAllComments();
    }
}
