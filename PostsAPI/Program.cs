using Core.Configuration;
using Core.Repositories;
using Core.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using PostsAPI.SyncDataServices.Grpc.Client;
using Infrastructure.Configurations;
using PostsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// JWT
builder.Services.AddSingleton<JwtSettings>();
builder.Services.ConfigureJwt(new JwtSettings(builder.Configuration));
builder.Services.AddScoped<IJwtService, JwtService>();

// Cors and swagger
builder.Services.ConfigureCors();
builder.Services.ConfigureSwagger();

// Database
var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
builder.Services.ConfigureMongo(mongoDbSettings);
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostService, PostService>();

// DataService
builder.Services.AddScoped<IGrpcCommentClient, GrpcCommentClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();