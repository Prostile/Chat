using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using Server.models.Decorator.Messages;
using Server.models.Decorator.Log;

namespace Server.hubs;

public record UserConnection(string UserName, string ChatId);

public interface IChatClient
{
    public Task ReceiveMessage(string message);
}

public class ChatHub : Hub<IChatClient>
{
    private readonly CustomMemoryCaches _cache;
    private readonly Server.models.Decorator.Log.ILogger _logger;

    public ChatHub(CustomMemoryCaches cache)
    {
        _cache = cache;
        _logger = new TimestampLogger(
                    new ConsoleLogger());
    }

    public async Task JoinChat(UserConnection connection)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatId);

        var stringConnection = JsonSerializer.Serialize(connection);

        // Сохраняем данные в кэше
        _cache.Set(Context.ConnectionId, stringConnection, TimeSpan.FromMinutes(30));

        await Clients
            .Group(connection.ChatId)
            .ReceiveMessage(MessageStringsFabric
                .MessageFromServerToClient($"{connection.UserName} подключился", "Admin"));
        _logger.Log($"{connection.UserName} подключился к чату {connection.ChatId}");
    }

    public async Task SendMessage(string message)
    {
        // Получаем данные из кэша
        if (_cache.TryGetValue(Context.ConnectionId, out var stringConnection))
        {
            var connection = JsonSerializer.Deserialize<UserConnection>(stringConnection.ToString());

            if (connection is not null)
            {
                _logger.Log($"получено {message} от {connection.UserName}");
                await Clients
                    .Group(connection.ChatId)
                    .ReceiveMessage(message);
                _logger.Log($"отправка {message} в чат {connection.ChatId}");
            }
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Получаем данные из кэша
        if (_cache.TryGetValue(Context.ConnectionId, out var stringConnection))
        {
            var connection = JsonSerializer.Deserialize<UserConnection>(stringConnection.ToString());

            if (connection is not null)
            {
                // Удаляем данные из кэша
                _cache.Remove(Context.ConnectionId);
                _logger.Log($"{connection.ChatId} удален из кэша");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.ChatId);

                await Clients
                    .Group(connection.ChatId)
                    .ReceiveMessage(MessageStringsFabric
                        .MessageFromServerToClient($"{connection.UserName} отключился", "Admin"));
                _logger.Log($"{connection.UserName} отключился от чата {connection.ChatId}");
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}