namespace ArianTel.Core.Abstractions;
public interface IDbBaseEntity
{
}

public interface IDbBaseEntity<TKey> : IDbBaseEntity
{
    TKey Id { get; set; }
}

public abstract class DbBaseEntity<TKey> : IDbBaseEntity<TKey>
{
    public TKey Id { get; set; }
}

public abstract class DbBaseEntity : DbBaseEntity<int>
{
}
