using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging
{
    public class RabbitMQMessageReceipt
    {
        private readonly string _hostName;
        private readonly int _port;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _virtualHost;
        private readonly string _queueName;
        private IConnection _connection;
        private IModel _channel;

        public event Action<string> MessageReceived;

        private IDictionary<string, object> MainQueueArgs =>
            new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", _queueName + ".retry" }
            };

        public RabbitMQMessageReceipt(string queueName = "fusion.doc.events")
        {
            _hostName = System.Configuration.ConfigurationManager.AppSettings["RabbitMQ:Host"]; ;
            _port = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RabbitMQ:Port"]);
            _userName = System.Configuration.ConfigurationManager.AppSettings["RabbitMQ:Username"];
            _password = System.Configuration.ConfigurationManager.AppSettings["RabbitMQ:Password"]; ;
            _virtualHost = System.Configuration.ConfigurationManager.AppSettings["RabbitMQ:VirtualHost"]; ;
            _queueName = queueName;
        }
        public void StartConsumer()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                Port = _port,
                UserName = _userName,
                Password = _password,
                DispatchConsumersAsync = false
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: MainQueueArgs);

            _channel.BasicQos(0, 1, false); // one message at a time

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, ea) =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(ea.Body.ToArray());

                    MessageReceived?.Invoke(message);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Logging.Log.Error(ex.Message,"RabbitMQMessageReceipt");
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: false);
                }
            };

            _channel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);
        }
        public void StopConsumer()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
