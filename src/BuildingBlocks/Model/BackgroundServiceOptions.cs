namespace BuildingBlocks.Model;


public class BackgroundServiceOptions
{
    public int SecondsInterval { get; set; }
    public bool IsActive { get; set; }
}
public sealed class BackgroundServiceOptionsDataBase : BackgroundServiceOptions
{
    public int FetchCount { get; set; }
    public int MaxAttemptCount { get; set; }
    public int InProgressTimeThreshold { get; set; }
    public int AdviceTimeRange { get; set; }
}
