using Grpc.Net.Client;

namespace PostsAPI.AsyncDataServices.Grpc.Client
{
    public class GrpcCommentClient : IGrpcCommentClient
    {
        private readonly IConfiguration _configuration;

        public GrpcCommentClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ReturnAllComments()
        {
            Console.WriteLine($"--> Calling GRPC Service {_configuration["GrpcComment"]}");
            var channel = GrpcChannel.ForAddress(_configuration["GrpcComment"]);
            var client = new GrpcComment.GrpcCommentClient(channel);
            var request = new GetAllRequest
            {
                Name = " Szymon"
            };

            try
            {
                var reply = client.GetAllComments(request);
                return reply.Comment;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Couldnot call GRPC Server {ex.Message}");
                return null;
            }
        }
    }
}
