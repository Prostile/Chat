using Microsoft.Extensions.Caching.Memory;
using Server.hubs;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ����������� SignalR
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.WriteIndented = true;
        options.PayloadSerializerOptions.IncludeFields = true;
        options.PayloadSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// ����������� IMemoryCache
builder.Services.AddMemoryCache();

// ����������� CustomMemoryCache
builder.Services.AddSingleton<CustomMemoryCaches>();

// ��������� CORS
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