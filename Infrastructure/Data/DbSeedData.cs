using Core.Entities.Models;
using Core.Enums;
using Core.Services;
using MongoDB.Driver;

namespace Infrastructure.Data;

public class DbSeedData
{
    public static void SeedUsers(IMongoCollection<User> collection)
    {
        var itemsNum = collection.EstimatedDocumentCount();
        if (itemsNum < 1) collection.InsertMany(GetSeedUsers());
    }
    
    public static void SeedPosts(IMongoCollection<Post> collection)
    {
        var itemsNum = collection.EstimatedDocumentCount();
        if (itemsNum < 1) collection.InsertMany(GetSeedPosts());
    }
    
    public static void SeedComments(IMongoCollection<Comment> collection)
    {
        var itemsNum = collection.EstimatedDocumentCount();
        if (itemsNum < 1) collection.InsertMany(GetSeedComments());
    }

    private static IEnumerable<Comment> GetSeedComments()
    {
        return new List<Comment>()
        {
            new Comment()
            {
                Id = new Guid(),
                UserId = new Guid("d4eecc03-c256-4b1d-961d-d7a520804d1a"),
                PostId = new Guid("d54a4a7a-a4ff-458f-abec-887eae5ae01e"),
                CreatedDate = DateTimeOffset.Now,
                Text = "Komentarz 1"
            },
            new Comment()
            {
                Id = new Guid(),
                UserId = new Guid("d4eecc03-c256-4b1d-961d-d7a520804d1a"),
                PostId = new Guid("d54a4a7a-a4ff-458f-abec-887eae5ae01e"),
                CreatedDate = DateTimeOffset.Now,
                Text = "Komentarz 2"
            },
            new Comment()
            {
                Id = new Guid(),
                UserId = new Guid("2cb18a07-a322-4b7b-9077-fb79dac84deb"),
                PostId = new Guid("d54a4a7a-a4ff-458f-abec-887eae5ae01e"),
                CreatedDate = DateTimeOffset.Now,
                Text = "Komentarz 3"
            }
        };
    }
    private static IEnumerable<User> GetSeedUsers()
    {

        var passwordService = new PasswordService();
        var password = passwordService.HashPassword("zaq1@WSX", out var salt);
        
        return new List<User>()
        {
            new User()
            {
                Id = new Guid("d4eecc03-c256-4b1d-961d-d7a520804d1a"),
                CreatedDate = DateTimeOffset.Now,
                Email = "user@example.pl",
                Role = Role.User,
                FirstName = "Janek",
                LastName = "Krzemień",
                PasswordHash = password,
                PasswordSalt = Convert.ToHexString(salt)
            },
            new User()
            {
                Id = new Guid("2cb18a07-a322-4b7b-9077-fb79dac84deb"),
                CreatedDate = DateTimeOffset.Now,
                Email = "admin@example.pl",
                Role = Role.Administrator,
                FirstName = "Paweł",
                LastName = "Skała",
                PasswordHash = password,
                PasswordSalt = Convert.ToHexString(salt)
            }
        };
    }
    private static IEnumerable<Post> GetSeedPosts()
    {
        return new List<Post>()
        {
            new Post()
            {
                Id = new Guid("d54a4a7a-a4ff-458f-abec-887eae5ae01e"),
                CreatedDate = DateTimeOffset.Now,
                Title = "Przykładowy tytuł",
                Text = "Ciekawy content",
                UserId = new Guid("2cb18a07-a322-4b7b-9077-fb79dac84deb"),
            },
            new Post()
            {
                Id = new Guid("08e74b65-4e6f-429f-aef9-929dad4bf724"),
                CreatedDate = DateTimeOffset.Now,
                Title = "Kolejny przykładowy tytuł",
                Text = "Jeszcze ciekawszy content",
                UserId = new Guid("d4eecc03-c256-4b1d-961d-d7a520804d1a"),
            }
        };
    }
}
