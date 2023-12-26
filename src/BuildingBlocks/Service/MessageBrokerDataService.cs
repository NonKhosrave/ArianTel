using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Logger;
using BuildingBlocks.Model;
using BuildingBlocks.Service.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace BuildingBlocks.Service;
public sealed class MessageBrokerDataService : IMessageBrokerDataService
{
    private readonly ILogger<MessageBrokerDataService> _logger;
    private readonly Dictionary<string, IConnection> _connections;
    private ConnectionFactory _factory;
    private readonly List<RabbitMqOptions> _rabbitMqOptions;

    public MessageBrokerDataService(IConfiguration configuration, ILogger<MessageBrokerDataService> logger)
    {
        _logger = logger;
        _rabbitMqOptions = configuration.GetSection("RabbitMqOptions")?.Get<List<RabbitMqOptions>>() ??
                           new List<RabbitMqOptions>();
        _connections = new Dictionary<string, IConnection>();

        ApplySettings();
    }

    public void Publish<T>(T data, string exchange, string routingKey, string connectionName)
    {
        if (!CanPublishToQueue(connectionName, exchange, routingKey))
        {
            throw new ArgumentException("connection or exchange or queue not found",
                $"{nameof(connectionName)}");
        }
        using var channel = GetIModelInstanceConnection(connectionName);
        var basicProperties = channel.CreateBasicProperties();
        var byteContent = data.ToJsonUtf8Bytes();
        basicProperties.Persistent = true;
        channel.BasicPublish(exchange, routingKey, true, basicProperties, byteContent);
        channel.Close();
    }

    public void BatchPublish(List<BatchPublishEntity> entities, string connectionName)
    {
        var firstEntity = entities.First();
        if (!CanPublishToQueue(connectionName, firstEntity.Exchange, firstEntity.RoutingKey))
        {
            throw new ArgumentException("connection or exchange or queue not found",
                $"{nameof(connectionName)}");
        }
        using var channel = GetIModelInstanceConnection(connectionName);
        var basicProperties = channel.CreateBasicProperties();
        basicProperties.Persistent = true;
        foreach (var entity in entities)
        {
            var byteContent = entity.ToJsonUtf8Bytes();
            channel.BasicPublish(entity.Exchange, entity.RoutingKey, true, basicProperties, byteContent);
        }

        channel.Close();
    }

    public RabbitMqOptions GetRabbitMqOptions(string connectionName) => _rabbitMqOptions.First(x =>
            x.ConnectionName.Equals(connectionName, StringComparison.OrdinalIgnoreCase));


    public RabbitExchangeOption GetExchange(string connectionName, string exchangeName) =>
        _rabbitMqOptions.First(x => x.ConnectionName.Equals(connectionName, StringComparison.OrdinalIgnoreCase)).Exchanges
            .First(x => x.Name.Equals(exchangeName, StringComparison.OrdinalIgnoreCase));


    public RabbitQueueOption GetQueue(string connectionName, string exchangeName, string queueName) => _rabbitMqOptions
        .First(x => x.ConnectionName.Equals(connectionName, StringComparison.OrdinalIgnoreCase)).Exchanges
        .First(x => x.Name.Equals(exchangeName, StringComparison.OrdinalIgnoreCase)).Queue
        .First(x => x.QueueName.Equals(queueName, StringComparison.OrdinalIgnoreCase));
    public void ApplySettings()

    {
        try
        {
            foreach (var rabbitMqOptions in _rabbitMqOptions)
            {
                if (string.IsNullOrEmpty(rabbitMqOptions.UserName) ||
                    string.IsNullOrEmpty(rabbitMqOptions.ConnectionName))
                    return;

                _factory ??= new ConnectionFactory();
                _factory.UserName = rabbitMqOptions.UserName;
                _factory.Password = rabbitMqOptions.Password;
                _factory.VirtualHost = rabbitMqOptions.VirtualHost;
                _factory.Port = rabbitMqOptions.HostOptions.First().Port;
                //_factory.AutomaticRecoveryEnabled = true;

                if (!_connections.ContainsKey(rabbitMqOptions.ConnectionName))
                {
                    var endpoints = rabbitMqOptions.HostOptions.ConvertAll(r => r.Host).ToList();
                    var connection = _factory.CreateConnection(endpoints);
                    _connections.Add(rabbitMqOptions.ConnectionName, connection);
                }

                if (!rabbitMqOptions.IsReadOnly)
                {
                    CreateQueues(rabbitMqOptions);
                }
            }

        }
        catch (Exception ex)
        {
            _logger.CompileLog(ex, LogLevel.Error,
                $"Failed to execute {nameof(MessageBrokerDataService)} with exception message {ex.Message}.");
        }
    }

    private void CreateQueues(RabbitMqOptions rabbitMqOptions)
    {
        using var model = GetIModelInstanceConnection(rabbitMqOptions.ConnectionName);
        foreach (var exchange in rabbitMqOptions.Exchanges)
        {
            if (!(exchange.Queue?.Any() ?? false))
                continue;

            model.ExchangeDeclare(exchange.Name, exchange.Type, exchange.Durable, exchange.AutoDelete);

            string exchangeDeadLetter = null;
            if (exchange.IsAddDeadLetter)
            {
                exchangeDeadLetter = $"{exchange.Name}Retry";
                model.ExchangeDeclare(exchangeDeadLetter, exchange.Type, exchange.Durable, exchange.AutoDelete);
            }

            foreach (var queue in exchange.Queue)
            {
                if (!queue.IsEnabled)
                    continue;

                model.QueueDeclare(queue.QueueName, queue.Durable, queue.Exclusive, queue.AutoDelete);

                if (queue.IsAddDeadLetter)
                {
                    var queueDeadLetter = $"{queue.QueueName}Retry";
                    var realDeadLetterExchangeName = exchangeDeadLetter ?? exchange.Name;
                    model.QueueDeclare(queueDeadLetter, queue.Durable, queue.Exclusive, queue.AutoDelete,
                        new Dictionary<string, object>
                        {
                                { "x-dead-letter-exchange", realDeadLetterExchangeName },
                                { "x-dead-letter-routing-key", queue.RoutingKey },
                                { "x-message-ttl", (int)TimeSpan.FromSeconds(queue.DelaySecond).TotalMilliseconds }
                        });

                    model.QueueBind(queueDeadLetter, exchangeDeadLetter, queue.RoutingKey);
                }

                model.QueueBind(queue.QueueName, exchange.Name, queue.RoutingKey);
            }
        }

        model.BasicQos(0, 1, false);
    }

    public IModel GetIModelInstanceConnection(string connectionName)
    {
        return _connections[connectionName].CreateModel();
    }


    private bool CanPublishToQueue(string connectionName, string exchangeName, string routingKey)
    {
        var canPublish = _rabbitMqOptions
            .FirstOrDefault(x => x.ConnectionName.Equals(connectionName, StringComparison.OrdinalIgnoreCase))?.Exchanges
            .FirstOrDefault(x => x.Name.Equals(exchangeName, StringComparison.OrdinalIgnoreCase))?.Queue
            .FirstOrDefault(x => x.RoutingKey.Equals(routingKey, StringComparison.OrdinalIgnoreCase));
        return canPublish is not null;
    }
}
