using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoinTest.Packets;

public class BalanceRequestPacket : AbstractPacket
{
    public readonly Guid uuid;
    public override PacketType PacketType
    {
        get { return PacketType.ClientBalance; }
    }

    public BalanceRequestPacket(Guid uuid) : base(DateTime.Now.Millisecond & 0xfffffff)
    {
        this.uuid = uuid;
    }

    protected override JToken? ToJsonData()
    {
        return uuid.ToString("N");
    }
}