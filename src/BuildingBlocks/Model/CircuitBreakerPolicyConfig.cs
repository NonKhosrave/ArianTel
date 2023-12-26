namespace BuildingBlocks.Model;

public sealed class CircuitBreakerPolicyConfig
{
    public CircuitBreakerPolicyConfig()
    {
        FailureDurationInSecond = 60;
        FailureThresholdPercent = 1;
        BreakDurationInSecond = 5;
        MinimumThroughOutput = 100;
    }


    /// <summary>
    ///     How Much Percents Of Requests Should Fail To Break
    /// </summary>
    public double FailureThresholdPercent { get; set; }

    /// <summary>
    ///     Period That Errors Measured
    /// </summary>
    public int FailureDurationInSecond { get; set; }

    /// <summary>
    ///     Minimum Request Count in Defined Period
    /// </summary>
    public int MinimumThroughOutput { get; set; }

    /// <summary>
    ///     Time To Break Circuit
    /// </summary>
    public int BreakDurationInSecond { get; set; }
}
