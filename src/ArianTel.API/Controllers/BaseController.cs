using System.Globalization;
using BuildingBlocks.Service;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ArianTel.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public abstract class BaseController : ControllerBase
{
    private IMediator _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    protected int GetUserId()
    {
        _ = int.TryParse(HttpContext.User.Identity?.GetUserId(), CultureInfo.InvariantCulture, out var userId);
        return userId;
    }
}
