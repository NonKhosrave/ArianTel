using System;
using System.ComponentModel.DataAnnotations;
using BuildingBlocks.Service;

namespace BuildingBlocks.Attributes;
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class PhoneNumberAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value == null)
            return false;

        if (value is not string valueStr)
            return false;

        return valueStr.IsValidMobilePhoneNumber();
    }
}
