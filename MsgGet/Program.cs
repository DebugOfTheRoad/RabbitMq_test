using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgGet
{
    class Program
    {
        private static readonly ConnectionFactory rabbitfactory = new ConnectionFactory { HostName = "127.0.0.1", UserName = "zhaogaolei", Password = "zhaogaolei", VirtualHost = "/" };
        const string ExchangeName = "test.exchange";
        const string QueueName = "test.queue";
        
        static void Main(string[] args)
        {
            IConnection connection = rabbitfactory.CreateConnection();
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(ExchangeName, "direct", durable: true, autoDelete: false, arguments: null);
                channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(QueueName, ExchangeName, routingKey: QueueName);
                var props = channel.CreateBasicProperties();
                props.SetPersistent(true);
               
                while(true)
                {
                    BasicGetResult msgResponse = channel.BasicGet(QueueName, noAck: false);
                    if(msgResponse==null)
                    {
                        break;
                    }
                    var msg = Encoding.UTF8.GetString(msgResponse.Body);
                    Console.WriteLine(msg);
                    channel.BasicAck(msgResponse.DeliveryTag, multiple: false);

                }
                Console.ReadLine();

            }
        }
    }
}
