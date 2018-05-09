using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace VKAggregator.Messaging
{
    class Rabbit
    {
        private IModel model;
        IBasicProperties basicProperties;
        public Rabbit()
        {
            RabbitMqService rabbitMqService = new RabbitMqService();
            IConnection connection = rabbitMqService.GetRabbitMqConnection();
            model = connection.CreateModel();

            basicProperties = model.CreateBasicProperties();
            basicProperties.SetPersistent(false);
        }


        /// <summary>
        /// Динамическая инициализация структуры обмена
        /// </summary>
        /// <param name="model"></param>
        private static void SetupInitialTopicQueue(IModel model)
        {
            model.QueueDeclare("queueFromVisualStudio", true, false, false, null);
            model.ExchangeDeclare("exchangeFromVisualStudio", ExchangeType.Topic);
            model.QueueBind("queueFromVisualStudio", "exchangeFromVisualStudio", "superstars");
        }

        public void Publish(String text)
        {
            byte[] payload = Encoding.UTF8.GetBytes(text);
            PublicationAddress address = new PublicationAddress(ExchangeType.Topic, "Vk_aggregator.exchange", "");
            model.BasicPublish(address, basicProperties, payload);
        }
    }
}
