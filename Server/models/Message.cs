namespace Server.models
{
    using System;
    using System.Text;
    using System.Text.Json.Serialization;

    namespace Decorator.Messages
    {
        public interface IMessage
        {
            string GetMessage();
        }

        public class Message : IMessage
        {
            private readonly string _content;

            public Message(string content)
            {
                _content = content;
            }

            public string GetMessage()
            {
                return _content;
            }
        }

        public abstract class MessageDecorator : IMessage
        {
            protected readonly IMessage _message;

            protected MessageDecorator(IMessage message)
            {
                _message = message;
            }

            public abstract string GetMessage();
        }

        public class EncryptedMessage : MessageDecorator
        {
            public EncryptedMessage(IMessage message) : base(message) { }

            public override string GetMessage()
            {
                return Encrypt(_message.GetMessage());
            }

            private string Encrypt(string message)
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
            }
        }
        public class DecryptedMessage : MessageDecorator
        {
            public DecryptedMessage(IMessage message) : base(message) { }

            public override string GetMessage()
            {
                return Decrypt(_message.GetMessage());
            }

            private string Decrypt(string message)
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(message));
            }
        }
        public class TimestampedMessage : MessageDecorator
        {
            public TimestampedMessage(IMessage message) : base(message) { }

            public override string GetMessage()
            {
                return $"{DateTime.Now}: {_message.GetMessage()}";
            }
        }
        public class PriorityMessage : MessageDecorator
        {
            private readonly string _priority;

            public PriorityMessage(IMessage message, string priority) : base(message)
            {
                _priority = priority;
            }

            public override string GetMessage()
            {
                return $"[{_priority}] {_message.GetMessage()}";
            }
        }
        public class WhoseMessage : MessageDecorator
        {
            private readonly string _author;

            public WhoseMessage(IMessage message, string author) : base(message)
            {
                _author = author;
            }

            public override string GetMessage()
            {
                return $"[{_author}] {_message.GetMessage()}";
            }
        }

        public static class MessageStringsFabric
        {
            public static string MessageToDecrypt(string message)
            {
                return new DecryptedMessage(
                          new Message(message))
                             .GetMessage();
            }
            public static string MessageToEncrypt(string message)
            {
                return new EncryptedMessage(
                          new Message(message))
                             .GetMessage();
            }

            public static string MessageFromClientToServer(string message, string priority)
            {
                return new EncryptedMessage(
                          new PriorityMessage(
                             new Message(message), priority))
                                .GetMessage();
            }
            public static string MessageFromServerToClient(string message, string autor)
            {
                return new TimestampedMessage(
                          new WhoseMessage(
                             new Message(message), autor))
                                .GetMessage();
            }
            public static string MessageFromClientToChat(string message)
            {
                return new TimestampedMessage(
                          new Message(message))
                             .GetMessage() + Environment.NewLine;
            }
            public static string MessageFromServerToChat(string message)
            {
                return new Message(message)
                          .GetMessage() + Environment.NewLine;
            }
        }

        public static class MessageObjectsFabric
        {
            public static IMessage MessageToDecrypt(string message)
            {
                return new DecryptedMessage(
                          new Message(message));
            }
            public static IMessage MessageToEncrypt(string message)
            {
                return new EncryptedMessage(
                          new Message(message));
            }

            public static IMessage MessageFromClientToServer(string message, string priority)
            {
                return new EncryptedMessage(
                          new PriorityMessage(
                             new Message(message), priority));
            }
            public static IMessage MessageFromServerToClient(string message, string autor)
            {
                return new TimestampedMessage(
                          new WhoseMessage(
                             new Message(message), autor));
            }
            public static IMessage MessageFromClientToChat(string message)
            {
                return new TimestampedMessage(
                          new Message(message));
            }
            public static IMessage Test1(string message, string priority, string author)
            {
                return new PriorityMessage(
                          new WhoseMessage(
                             new Message(message), author), priority);
            }
            public static IMessage Test2(string message, string priority)
            {
                return new PriorityMessage(
                             new Message(message), priority);
            }
        }
    }

    namespace Decorator.Log
    {
        public interface ILogger
        {
            void Log(string message);
        }

        public class ConsoleLogger : ILogger
        {
            public void Log(string message)
            {
                Console.WriteLine(message);
            }
        }

        public abstract class LoggerDecorator : ILogger
        {
            protected readonly ILogger _logger;

            protected LoggerDecorator(ILogger logger)
            {
                _logger = logger;
            }

            public abstract void Log(string message);
        }

        public class TimestampLogger : LoggerDecorator
        {
            public TimestampLogger(ILogger logger) : base(logger) { }

            public override void Log(string message)
            {
                string timestampedMessage = $"{DateTime.Now}: {message}";
                _logger.Log(timestampedMessage);
            }
        }
        public class ErrorLogger : LoggerDecorator
        {
            public ErrorLogger(ILogger logger) : base(logger) { }

            public override void Log(string message)
            {
                string errorMessage = $"ERROR: {message}";
                _logger.Log(errorMessage);
            }
        }
    }

}
