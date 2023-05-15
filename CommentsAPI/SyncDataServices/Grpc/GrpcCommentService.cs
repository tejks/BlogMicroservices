using Grpc.Core;

namespace CommentsAPI.SyncDataServices.Grpc
{
    public class GrpcCommentService : GrpcComment.GrpcCommentBase
    {
        public override Task<CommentResponse> GetAllComments(GetAllRequest request, ServerCallContext context)
        {
            var response = new CommentResponse
            {
                Comment = "Hello" + request.Name
            };

            return Task.FromResult(response);
        }
    }
}