using System;
using System.Collections.Generic;
using BuildingBlocks.Domain.Services;
using BuildingBlocks.Model;

namespace BuildingBlocks.Service;
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
public sealed class CustomException : Exception
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
{
    public CustomException(string exceptionCode, string message) : base(message)
    {
        Error = new ErrorDto(exceptionCode, message);
    }

#pragma warning disable S3427 // Method overloads with default parameter values should not overlap
    public CustomException(string exceptionCode, string target, string message = null) : base(message)
#pragma warning restore S3427 // Method overloads with default parameter values should not overlap
    {
        Error = new ErrorDto(exceptionCode, message, target);
    }

    public CustomException(string exceptionCode, string target, string message,
        List<ValidationError> errorDetails) : base(
        message)
    {
        Error = new ErrorDto(exceptionCode, message, target, errorDetails);
    }

    public CustomException(ErrorDto error)
    {
        Error = error;
    }

    public CustomException()
    {
    }

    public CustomException(string message) : base(message)
    {
        Error = new ErrorDto(message, message);
    }

    public CustomException(string message, Exception innerException) : base(message, innerException)
    {
        Error = new ErrorDto(message, message);
    }

    public ErrorDto Error { get; }
}
