using System.Collections.Generic;
using BuildingBlocks.Domain.Services;

namespace BuildingBlocks.Model;
public readonly struct ErrorDto
{
    public ErrorDto(ValidationError error)
    {
        Error = error;
    }


    public ErrorDto(string code, string message)
    {
        Error = new ValidationError(code, message);
    }

    public ErrorDto(string code, string message, string target)
    {
        Error = new ValidationError(code, message, target);
    }

    public ErrorDto(string code, string message, string target, int httpCode)
    {
        Error = new ValidationError(code, message, target, httpCode);
    }

    public ErrorDto(string code, string message, string target, List<ValidationError> details)
    {
        Error = new ValidationError(code, message, target, details);
    }

    public ValidationError Error { get; init; }
}
