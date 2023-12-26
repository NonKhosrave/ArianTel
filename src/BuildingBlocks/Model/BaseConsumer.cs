namespace BuildingBlocks.Model;
public abstract class BaseConsumer
{
    public string QueueName { get; set; }
    public string ExchangeName { get; set; }
    public bool IsEnabled { get; set; }
}

public abstract class BaseJobConfig
{
    public bool Enabled { get; set; }
    public int ProcessTimeRangeSecond { get; set; }
    public int TimeIntervalSecond { get; set; }
    public int TopCountForProcess { get; set; }
    public int InProgressTimeThresholdSecond { get; set; }
    public int ProcessMaxAttempt { get; set; }
}
