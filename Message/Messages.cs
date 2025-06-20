﻿namespace Message
{
    
    using Message.Decorator.Messages.DTO;
    using System;
    using System.Text;

    namespace Decorator.Messages
    {
        public interface IMessage
        {
            string GetMessage();
            void Serialize(MessageDTOConverter msg);
        }

        public interface ISerializable
        {
        }

        public class Message : IMessage
        {
            private readonly string _content;

            public Message(string content, string additional = null)
            {
                _content = content;
            }

            public string GetMessage()
            {
                return _content;
            }

            public void Serialize(MessageDTOConverter dto)
            {
                dto.DTO.Content = _content;
                dto.AddType(this);
            }
        }

        public abstract class MessageDecorator : IMessage
        {
            protected readonly IMessage _message;

            protected MessageDecorator(IMessage message, string additional = null)
            {
                _message = message;
            }

            public abstract string GetMessage();
            public abstract void Serialize(MessageDTOConverter dto);
        }

        public class EncryptedMessage : MessageDecorator
        {
            public EncryptedMessage(IMessage message, string additional = null) : base(message) { }

            public override string GetMessage()
            {
                return Encrypt(_message.GetMessage());
            }
            public override void Serialize(MessageDTOConverter dto)
            {
                _message.Serialize(dto);
                dto.AddType(this);
            }

            private string Encrypt(string message)
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
            }
        }
        public class DecryptedMessage : MessageDecorator
        {
            public DecryptedMessage(IMessage message, string additional = null) : base(message) { }

            public override string GetMessage()
            {
                return Decrypt(_message.GetMessage());
            }
            public override void Serialize(MessageDTOConverter dto)
            {
                _message.Serialize(dto);
                dto.AddType(this);
            }
            private string Decrypt(string message)
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(message));
            }
        }
        public class TimestampedMessage : MessageDecorator
        {
            public TimestampedMessage(IMessage message, string additional = null) : base(message) { }

            public override string GetMessage()
            {
                return $"{DateTime.Now}: {_message.GetMessage()}";

            }
            public override void Serialize(MessageDTOConverter dto)
            {
                _message.Serialize(dto);
                dto.AddType(this);
            }
        }
        public class PriorityMessage : MessageDecorator
        {
            private readonly string _priority;

            public PriorityMessage(IMessage message, string additional = null) : base(message)
            {
                _priority = additional;
            }

            public override string GetMessage()
            {
                return $"[{_priority}] {_message.GetMessage()}";
            }
            public override void Serialize(MessageDTOConverter dto)
            {
                dto.DTO.Priority = _priority;
                _message.Serialize(dto);
                dto.AddType(this);
            }
        }
        public class WhoseMessage : MessageDecorator
        {
            private readonly string _author;

            public WhoseMessage(IMessage message, string additional = null) : base(message)
            {
                _author = additional;
            }

            public override string GetMessage()
            {
                return $"[{_author}] {_message.GetMessage()}";
            }
            public override void Serialize(MessageDTOConverter dto)
            {
                dto.DTO.Author = _author;
                _message.Serialize(dto);
                dto.AddType(this);
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

    namespace Decorator.Messages.DTO
    {
        public class WrapDTO
        {
            public MessageDTO Message { get; set; }
            public WrapDTO(MessageDTO m) { Message = m; }
        }
        public class MessageDTO
        {
            public string Content { get; set; } = "";
            public string Priority { get; set; } = "";
            public string Author { get; set; } = "";
            public List<string> Decorators { get; set; } = new List<string>();
        }

        public class MessageDTOConverter
        {
            public MessageDTO DTO { get; set; } = new MessageDTO();
            public MessageDTOConverter(IMessage msg)
            {
                msg.Serialize(this);
            }

            public MessageDTOConverter(MessageDTO dto)
            {
                DTO = dto;
            }

            public void AddType(object o)
            {
                if (o is IMessage)
                {
                    DTO.Decorators.Add(o.GetType().Name); // Добавляем имя типа как строку
                }
            }

            public IMessage CreateMessage()
            {
                IMessage msg = new Message(DTO.Content); // Создаем базовое сообщение

                foreach (var decoratorName in DTO.Decorators)
                {
                    if (decoratorName == nameof(PriorityMessage)) // Используем nameof для получения имени типа
                    {
                        msg = new PriorityMessage(msg, DTO.Priority);
                    }
                    else if (decoratorName == nameof(WhoseMessage))
                    {
                        msg = new WhoseMessage(msg, DTO.Author);
                    }
                    else if (decoratorName == nameof(EncryptedMessage))
                    {
                        msg = new EncryptedMessage(msg);
                    }
                    else if (decoratorName == nameof(DecryptedMessage))
                    {
                        msg = new DecryptedMessage(msg);
                    }
                    else if (decoratorName == nameof(TimestampedMessage))
                    {
                        msg = new TimestampedMessage(msg);
                    }
                }

                return msg;
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
