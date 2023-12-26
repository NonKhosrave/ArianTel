namespace BuildingBlocks;

public readonly record struct KeyValueObj(string Key, string Value, string PersianKey = null, bool Copyable = false);
