using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ArianTel.API.ValidationAttributeHelper;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AtLeastOneParameterAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var obj = value;
        var properties = obj.GetType().GetProperties();

        var hasValue = properties.Any(property => property.GetValue(obj) != null);

        return !hasValue ? new ValidationResult("At least one parameter must be provided.", new[] { validationContext.DisplayName }) : ValidationResult.Success;
    }
}
