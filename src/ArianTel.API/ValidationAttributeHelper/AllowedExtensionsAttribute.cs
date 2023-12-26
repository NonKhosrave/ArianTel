using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ArianTel.API.ValidationAttributeHelper;

[System.AttributeUsage(System.AttributeTargets.All)]
public sealed class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;
    public AllowedExtensionsAttribute(string[] extensions)
    {
        _extensions = extensions;
    }

    protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            if (!((IList)_extensions).Contains(extension.ToLower(CultureInfo.InvariantCulture)))
            {
                return new ValidationResult(ErrorMessage());
            }
        }

        return ValidationResult.Success;
    }

    public new string ErrorMessage()
    {
        return $"This photo extension is not allowed! Please select .jpg/.jpeg/.png files only";
    }
}
