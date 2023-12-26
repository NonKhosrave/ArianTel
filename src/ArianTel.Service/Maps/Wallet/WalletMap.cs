using System.Collections.Generic;
using ArianTel.Service.Commands.Wallet;
using ArianTel.Service.Commands.Wallet.Core;
using Mapster;

namespace ArianTel.Service.Maps.Wallet;
public sealed class WalletMap : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<List<TransactionItem>, GetTransactionReportCommandResponse>()
            .Map(r => r.Items, r => r);

        config
            .NewConfig<ChargeByReferenceWalletCommandRequest, TransferBalanceCommandRequest>()
            .Ignore(r => r.AfterNextEvents)
            .Ignore(r => r.BeforeNextEvents);
    }
}
