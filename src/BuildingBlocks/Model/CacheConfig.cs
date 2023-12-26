using System.Collections.Generic;

namespace BuildingBlocks.Model;

public sealed class CacheConfig
{
    public bool IsEnabled { get; set; }

    public RedisCache RedisCache { get; set; }

    //public Couchbase Couchbase { get; set; }
    public InMemoryCache InMemory { get; set; }
}

public sealed class InMemoryCache
{
    public bool IsEnabled { get; set; }
}

public sealed class RedisCache
{
    public RedisCache()
    {
        Connection = new List<string>();
    }

    public List<string> Connection { get; set; }
    public string InstanceName { get; set; }
    public bool IsEnabled { get; set; }
    public string Password { get; set; }
    public string ServiceName { get; set; }
}

public sealed class Couchbase
{
    public Couchbase()
    {
        Servers = new List<string>();
    }

    public string Username { get; set; }
    public string Password { get; set; }
    public List<string> Servers { get; set; }
    public string BucketName { get; set; }
    public bool IsEnabled { get; set; }
}
