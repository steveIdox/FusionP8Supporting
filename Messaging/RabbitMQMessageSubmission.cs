using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging
{
    public class RabbitMQMessageSubmission
    {
        private readonly string _hostName;
        private readonly int _port;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _virtualHost;
        private readonly string _queueName;
        private IConnection _connection;
        private IModel _channel;

        private IDictionary<string, object> MainQueueArgs =>
            new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", _queueName + ".retry" }
            };
        private IDictionary<string, object> RetryQueueArgs =>
            new Dictionary<string, object>
            {
                { "x-message-ttl", 30000 },
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", _queueName }
            };

        public RabbitMQMessageSubmission(
            string rabbitMQHost,
            int rabbitMQPort,
            string rabbitMQUsername,
            string rabbitMQPassword,
            string rabbitMQVirtualHost,
            string queueName = "fusion.doc.events")
        {
            _hostName = rabbitMQHost;
            _port = rabbitMQPort;
            _userName = rabbitMQUsername;
            _password = rabbitMQPassword;
            _virtualHost = rabbitMQVirtualHost;
            _queueName = queueName;

            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                Port = _port,
                UserName = _userName,
                Password = _password,
                VirtualHost = _virtualHost
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ConfirmSelect();
            _channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: MainQueueArgs);

            // RETRY QUEUE (delayed)
            _channel.QueueDeclare(
                queue: _queueName + ".retry",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: RetryQueueArgs);

            // DEAD LETTER QUEUE
            _channel.QueueDeclare(
                queue: _queueName + ".dlq",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public void PostMessage(string queueName, string message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                Port = _port,
                UserName = _userName,
                Password = _password,
                VirtualHost = _virtualHost
            };

            _channel.ConfirmSelect();

            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: MainQueueArgs);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: false,
                basicProperties: properties,
                body: body);

            _channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
        }

        public void PostMessageBatch(string queueName, IEnumerable<string> messages)
        {

            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                Port = _port,
                UserName = _userName,
                Password = _password,
                VirtualHost = _virtualHost
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ConfirmSelect();

            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: MainQueueArgs);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            foreach (var message in messages)
            {
                var body = Encoding.UTF8.GetBytes(message);
                _channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: queueName,
                    mandatory: false,
                    basicProperties: properties,
                    body: body);
            }
        }
    }
}