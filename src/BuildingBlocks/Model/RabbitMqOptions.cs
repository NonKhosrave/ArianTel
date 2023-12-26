using System.Collections.Generic;

namespace BuildingBlocks.Model;
public sealed class BatchPublishEntity
{
    public object Data { get; set; }
    public string Exchange { get; set; }
    public string RoutingKey { get; set; }
}

public sealed class RabbitMqOptions
{
    public RabbitMqOptions()
    {
        HostOptions = new List<HostOptions>();
        Exchanges = new List<RabbitExchangeOption>();
    }

    public List<HostOptions> HostOptions { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string VirtualHost { get; set; } = "/";
    public List<RabbitExchangeOption> Exchanges { get; set; }
    public bool IsReadOnly { get; set; }
    public string ConnectionName { get; set; }
}

public sealed class RabbitExchangeOption
{
    public RabbitExchangeOption()
    {
        Queue = new List<RabbitQueueOption>();
    }

    public string Name { get; set; }
    public string Type { get; set; }
    public bool Durable { get; set; } = true;
    public bool AutoDelete { get; set; }
    public bool IsAddDeadLetter { get; set; } = true;
    public List<RabbitQueueOption> Queue { get; set; }
}

public sealed class RabbitQueueOption
{
    public string QueueName { get; set; }
    public string RoutingKey { get; set; }
    public bool IsEnabled { get; set; }
    public int DelaySecond { get; set; }
    public bool Durable { get; set; } = true;
    public bool Exclusive { get; set; }
    public bool AutoDelete { get; set; } = true;
    public bool IsAddDeadLetter { get; set; } = true;
    public ushort PreFetchCount { get; set; }

}

public sealed class HostOptions
{
    public string Host { get; set; }
    public int Port { get; set; }
}
