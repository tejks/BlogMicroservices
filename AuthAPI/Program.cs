using System.Text.Json.Serialization;
using AuthAPI.AsyncDataService;
using AuthAPI.Models;
using AuthAPI.Services;
using Core.Configuration;
using Core.Entities.Models;
using Core.Repositories;
using Core.Services;
using Infrastructure.Configurations;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// JWT
builder.Services.AddSingleton<JwtSettings>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.ConfigureJwt(new JwtSettings(builder.Configuration));

// Cors and swagger
builder.Services.ConfigureCors();
builder.Services.ConfigureSwagger();

// Database
var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
builder.Services.ConfigureMongo(mongoDbSettings);
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

// Model services
builder.Services.AddScoped<IUserService, UserService>();

// Tool services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

builder.Services.AddScoped<IMessageBusAuthClient, MessageBusAuthClient>();

BsonClassMap.RegisterClassMap<User>(cm =>
{
    cm.AutoMap();
    cm.UnmapMember(m => m.RefreshTokens);
});

BsonClassMap.RegisterClassMap<RefreshToken>(cm =>
{
    cm.AutoMap();
    cm.UnmapMember(m => m.User);
});

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