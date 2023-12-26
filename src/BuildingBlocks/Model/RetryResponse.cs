using System;

namespace BuildingBlocks.Model;

public sealed class RetryResponse<T>
{
    public bool IsSuccessful { get; set; }

    public T Result { get; set; }

    public Exception Exception { get; set; }
}
