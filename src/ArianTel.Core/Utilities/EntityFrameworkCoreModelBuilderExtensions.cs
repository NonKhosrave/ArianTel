using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ArianTel.Core.Utilities;
public static class EntityFrameworkCoreModelBuilderExtensions
{
    public static void RegisterAllEntities<TBaseType>(this ModelBuilder modelBuilder, params Assembly[] assemblies)
    {
        var types = assemblies.SelectMany(a => a.GetExportedTypes())
            .Where(c => c is { IsClass: true, IsAbstract: false, IsPublic: true } && typeof(TBaseType).IsAssignableFrom(c));

        foreach (var type in types)
            modelBuilder.Entity(type);
    }

    public static void SetDecimalPrecision(this ModelBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        foreach (var property in builder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal)
                                 || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18, 6)");
        }
    }

    public static void AddDateTimeUtcKindConverter(this ModelBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        // If you store a DateTime object to the DB with a DateTimeKind of either `Utc` or `Local`,
        // when you read that record back from the DB you'll get a DateTime object whose kind is `Unspecified`.
        // Here is a fix for it!
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => !v.HasValue ? v : ToUniversalTime(v),
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var property in builder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties()))
        {
            if (property.ClrType == typeof(DateTime))
            {
                property.SetValueConverter(dateTimeConverter);
            }

            if (property.ClrType == typeof(DateTime?))
            {
                property.SetValueConverter(nullableDateTimeConverter);
            }
        }
    }

    private static DateTime? ToUniversalTime(DateTime? dateTime)
    {
        if (!dateTime.HasValue)
        {
            return null;
        }

        return dateTime.Value.Kind == DateTimeKind.Utc ? dateTime : dateTime.Value.ToUniversalTime();
    }
}
