using System;

namespace BuildingBlocks.DateTimeHelper;
public interface IDateTimeProvider
{
    DateTime GetNow();

    DateTime GetUtcNow();

    DateTimeOffset GetNowOffset();

    DateTimeOffset GetUtcNowOffset();

}
