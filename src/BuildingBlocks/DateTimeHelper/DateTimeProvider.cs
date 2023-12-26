using System;

namespace BuildingBlocks.DateTimeHelper;
public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime GetNow() => DateTime.Now;

    public DateTime GetUtcNow() => DateTime.UtcNow;

    public DateTimeOffset GetNowOffset() => DateTimeOffset.Now;

    public DateTimeOffset GetUtcNowOffset() => DateTimeOffset.UtcNow;

}
