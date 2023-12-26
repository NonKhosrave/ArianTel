using System;
using System.ComponentModel.DataAnnotations;
using BuildingBlocks.Service;

namespace BuildingBlocks.Attributes;
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class ValidIranianPostalCodeAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value == null)
            return true;
        var postalCode = value.ToString();
        return string.IsNullOrWhiteSpace(postalCode) || postalCode.IsValidIranianPostalCode();
    }
}
