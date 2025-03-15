using Microsoft.Extensions.Caching.Memory;
using Server.hubs;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Регистрация SignalR
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.WriteIndented = true;
        options.PayloadSerializerOptions.IncludeFields = true;
        options.PayloadSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Регистрация IMemoryCache
builder.Services.AddMemoryCache();

// Регистрация CustomMemoryCache
builder.Services.AddSingleton<CustomMemoryCaches>();

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors();

app.MapHub<ChatHub>("/chat");

app.Run();