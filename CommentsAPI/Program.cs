using CommentsAPI.AsyncDataService;
using CommentsAPI.EventProcessing;
using Core.Configuration;
using Core.Repositories;
using Core.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using CommentsAPI.Services;
using CommentsAPI.SyncDataServices.Grpc;
using Infrastructure.AsyncDataServices;
using Infrastructure.Configurations;

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
builder.Services.AddTransient<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddTransient<IEventProcessor, EventProcessor>(); 

// DataService
builder.Services.AddHostedService<MessageBusCommentSubscriber>();
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed database
var context = app.Services.GetRequiredService<IMongoDbContext>();
DbSeedData.SeedComments(context.Comments);

app.MapGrpcService<GrpcCommentService>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();