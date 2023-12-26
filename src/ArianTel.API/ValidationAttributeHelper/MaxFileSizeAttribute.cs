using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ArianTel.API.ValidationAttributeHelper;

[System.AttributeUsage(System.AttributeTargets.All)]
public sealed class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly int _maxFileSize;
    public MaxFileSizeAttribute(int maxFileSize)
    {
        _maxFileSize = maxFileSize;
    }

    protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
    {
        if (value is IFormFile file && file.Length > _maxFileSize)
        {
            return new ValidationResult(ErrorMessage());
        }

        return ValidationResult.Success;
    }

    public new string ErrorMessage() => $"Maximum allowed file size is {_maxFileSize} bytes.";
}
