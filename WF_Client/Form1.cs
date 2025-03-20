using Microsoft.AspNetCore.SignalR.Client;
using Client.models.Decorator.Messages;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Client.models.Decortor.Messages.DTO;

public record UserConnection(string UserName, string ChatId);

namespace Client
{
    public partial class ChatForm : Form
    {
        private HubConnection _connection;
        private string _chatId = "defaultChat"; // Замените на ваш идентификатор чата

        public ChatForm()
        {
            InitializeComponent();
            InitializeSignalR();
        }

        private async void InitializeSignalR()
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5281/chat")
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions = jsonOptions;
                })
                .Build();

            _connection.On<string>("ReceiveMessage", (message) =>
            {
                Invoke(new Action(() =>
                {
                    listBoxMessages.Items.Add(message);
                }));
            });

            await _connection.StartAsync();
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            var userName = textBoxUserName.Text;
            var message = textBoxMessage.Text;

            IMessage msg = MessageStringsFabric.MessageFromClientToChat(message);
            msg = (new MessageDTO(msg)).CreateMessage();

            MessageBox.Show(msg.GetMessage);

            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(message))
            {
                await _connection.InvokeAsync("SendMessage", MessageStringsFabric
                                    .MessageFromClientToChat(message));
                textBoxMessage.Clear();
            }
        }

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _connection.StopAsync();
        }

        private async void buttonAccept_Click(object sender, EventArgs e)
        {
            var userName = textBoxUserName.Text;

            if (!string.IsNullOrWhiteSpace(userName))
            {
                buttonSend.Enabled = true;
                buttonAccept.Enabled = false;
                var connection = new UserConnection(userName, _chatId);
                await _connection.InvokeAsync("JoinChat", connection);
            }
            else
            {
                MessageBox.Show("Необходимо ввести имя");
            }
        }
    }
}