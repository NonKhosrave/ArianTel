using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ArianTel.API.Models.Ipg;
using ArianTel.Service.Commands.Ipg;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace ArianTel.API.Controllers.v1.Ipg;
[ApiVersion("1")]
public sealed class IpgController : BaseController
{
    [HttpPost("InitIpg")]
    //[Authorize(Roles = ConstantRoles.Customer)]
    public async Task<IActionResult> InitIpgAsync(InitIpgRequest model, CancellationToken cancellationToken)
    {
        var command = model.Adapt<InitIpgCommandRequest>();
        command.UserId = GetUserId();
        command.CustomerIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        command.UserAgent = Request.Headers["User-Agent"].ToString();
        var result = await Mediator.Send(command, cancellationToken);
        if (result.HasError)
            return BadRequest(result.Error);

        return Ok(result);
    }

    [HttpPost("CallBackFromSaman")]
    public async Task<IActionResult> CallBackFromSaman([FromForm] CallBackFromSamanRequest request)
    {
        var command = request.Adapt<CallBackCommandRequest>();
        command.TerminalInitIpgRequestId =
            long.TryParse(request.ResNum, CultureInfo.InvariantCulture, out var baseIpgInfoId)
                ? baseIpgInfoId
                : 0;
        command.CustomerIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await Mediator.Send(command);
        return Redirect(result.RedirectUrl);
    }
}
