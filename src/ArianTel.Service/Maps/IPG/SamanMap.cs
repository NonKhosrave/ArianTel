using System;
using ArianTel.Core.Services.Models.Ipg;
using ArianTel.Service.Commands.Ipg;
using ArianTel.Service.Services.Models.Ipg;
using Mapster;

namespace ArianTel.Service.Maps.Ipg;
public sealed class SamanMap : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<InitIpgCommandRequest, InitIpgModelRequest>()
            .Map(des => des.TrackingCode, src => Guid.NewGuid().ToString("N"));

        config
            .NewConfig<InitIpgModelRequest, InitIpgProviderRequest>()
            .Map(r => r.CellNumber, r => r.PhoneNumber)
            .Map(r => r.ResNum, r => r.TerminalInitIpgRequestId);
    }
}
