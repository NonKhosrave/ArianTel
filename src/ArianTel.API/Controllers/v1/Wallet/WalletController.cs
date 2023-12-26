using System.Threading;
using System.Threading.Tasks;
using ArianTel.API.Models.Wallet;
using ArianTel.Service.Commands.Wallet;
using ArianTel.Service.Commands.Wallet.Core;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace ArianTel.API.Controllers.v1.Wallet;

[ApiVersion("1")]
public sealed class WalletController : BaseController
{
    [HttpGet("GetBalanceById/{id:int}")]
    //[Authorize(Roles = $"{ConstantRoles.Admin},{ConstantRoles.Vendor}")]
    public async Task<IActionResult> GetBalanceByIdAsync(int id, CancellationToken cancellationToken)
    {
        var commandRequest = new GetBalanceByIdCommandRequest() { UserId = id };

        var result = await Mediator.Send(commandRequest, cancellationToken);

        if (result.HasError)
            return BadRequest(result.Error);

        return Ok(result);
    }

    [HttpGet("GetTransactionReport")]
    //[Authorize(Roles = ConstantRoles.Admin)]
    public async Task<IActionResult> GetTransactionReportAsync([FromQuery] TransactionReportDto transactionReportDto, CancellationToken cancellationToken)
    {
        var commandRequest = transactionReportDto.Adapt<GetTransactionReportCommandRequest>();

        var result = await Mediator.Send(commandRequest, cancellationToken);
        if (result.HasError)
            return BadRequest(result.Error);

        return Ok(result);
    }

    [HttpPost("ChargeByReferenceWallet/{id:int}")]
    //[Authorize(Roles = ConstantRoles.Admin)]
    public async Task<IActionResult> ChargeByReferenceWalletAsync(int id, [FromForm] BaseWalletDto baseWalletDto, CancellationToken cancellationToken)
    {
        var commandRequest = baseWalletDto.Adapt<ChargeByReferenceWalletCommandRequest>();
        commandRequest.DesUserId = id;

        var result = await Mediator.Send(commandRequest, cancellationToken);
        if (result.HasError)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpPost("ChargeReferenceWallet")]
    //[Authorize(Roles = ConstantRoles.Admin)]
    public async Task<IActionResult> ChargeReferenceAsync([FromForm] BaseWalletDto baseWalletDto, CancellationToken cancellationToken)
    {
        var commandRequest = baseWalletDto.Adapt<ChargeReferenceWalletCommandRequest>();

        var result = await Mediator.Send(commandRequest, cancellationToken);
        if (result.HasError)
            return BadRequest(result.Error);

        return NoContent();
    }
}
