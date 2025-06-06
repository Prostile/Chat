using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Message.Decorator.Messages.DTO;
using Message.Decorator.Messages;

public record UserConnection(string UserName, string ChatId);

namespace Client
{
    public partial class ChatForm : Form
    {
        private HubConnection _connection;
        private string _chatId = "defaultChat"; // идентификатор чата

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

            _connection.On<string>("ReceiveMessage", message =>
                {
                    Invoke(new Action(() =>
                    {
                        var msg = message;
                        listBoxMessages.Items.Add(msg);
                    }));
                });

            await _connection.StartAsync();
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            var userName = textBoxUserName.Text;
            var message = textBoxMessage.Text;

            string msg = MessageStringsFabric.MessageToEncrypt(message);
            MessageBox.Show(msg);
            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(message))
            {
                try
                {
                    await _connection.InvokeAsync("SendMessage", msg);
                    textBoxMessage.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}");
                }
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
                try
                {
                    await _connection.InvokeAsync("JoinChat", connection);
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"{ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Необходимо ввести имя");
            }
        }
    }
}