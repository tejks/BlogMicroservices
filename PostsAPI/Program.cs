using Core.Entities.Models;
using Core.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using PostsAPI.AsyncDataServices.Grpc.Client;
using AutoMapper;
using Amazon.Runtime.Internal.Util;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGrpcCommentClient, GrpcCommentClient>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));


builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    return new MongoClient("mongodb://mongo_db:27017");
});

BsonClassMap.RegisterClassMap<Post>(cm =>
{
    cm.AutoMap();
    cm.UnmapMember(m => m.Comments);
    cm.UnmapMember(m => m.User);
});

BsonClassMap.RegisterClassMap<User>(cm =>
{
    cm.AutoMap();
    cm.UnmapMember(m => m.Comments);
    cm.UnmapMember(m => m.Posts);
});

BsonClassMap.RegisterClassMap<Comment>(cm =>
{
    cm.AutoMap();
    cm.UnmapMember(m => m.User);
    cm.UnmapMember(m => m.Post);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();