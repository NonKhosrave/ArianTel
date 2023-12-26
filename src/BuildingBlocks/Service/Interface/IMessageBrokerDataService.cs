using System.Collections.Generic;
using BuildingBlocks.Model;
using RabbitMQ.Client;

namespace BuildingBlocks.Service.Interface;
public interface IMessageBrokerDataService
{
    IModel GetIModelInstanceConnection(string connectionName);
    void Publish<T>(T data, string exchange, string routingKey, string connectionName);
    void BatchPublish(List<BatchPublishEntity> entities, string connectionName);
    RabbitMqOptions GetRabbitMqOptions(string connectionName);
    RabbitExchangeOption GetExchange(string connectionName, string exchangeName);
    RabbitQueueOption GetQueue(string connectionName, string exchangeName, string queueName);
}
