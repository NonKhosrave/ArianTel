using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuildingBlocks.Filter;
/// <summary>
///     FilterAttributes Extensions
/// </summary>
public static class FilterAttributesExtensions
{
    /// <summary>
    ///     Cleans all of the string values of the current ActionArguments and model's stringProperties
    /// </summary>

    private static readonly Type StringType = typeof(string);

    private static readonly Type CancellationTokenType = typeof(CancellationToken);
    private static readonly Type ListType = typeof(List<>);

    public static void CleanupActionStringValues(this ActionExecutingContext context, Func<string, string> action)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        foreach (var (aKey, aValue) in context.ActionArguments)
        {
            if (aValue is null)
            {
                continue;
            }

            var type = aValue.GetType();

            if (type == CancellationTokenType)
            {
                continue;
            }

            if (type == StringType)
            {
                context.ActionArguments[aKey] = action(aValue.ToString() ?? "");
            }
            else if ((type.IsArray && type.GetElementType() == StringType) ||
                     (type.IsGenericType && type.GetGenericTypeDefinition() == ListType &&
                      type.GenericTypeArguments[0] == StringType))
            {
                var items = ((IEnumerable)aValue).OfType<string>().Select(action).ToList();
                context.ActionArguments[aKey] = type.IsArray ? items.ToArray() : items;
            }
            else
            {
                var stringProperties = type
                                             .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                             .Where(x =>
                                                        x.CanRead
                                                        && x.CanWrite
                                                        && x.PropertyType == StringType
                                                        && x.GetGetMethod(true)?.IsPublic == true
                                                        && x.GetSetMethod(true)?.IsPublic == true
                                                        && !x.GetIndexParameters().Any());

                foreach (var propertyInfo in stringProperties)
                {
                    var value = propertyInfo.GetValue(aValue);
                    if (value is null)
                    {
                        continue;
                    }

                    propertyInfo.SetValue(aValue, action(value.ToString() ?? ""));
                }
            }
        }
    }
}
