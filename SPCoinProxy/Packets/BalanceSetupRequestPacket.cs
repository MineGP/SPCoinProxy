using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoinTest.Packets;

public class BalanceSetupRequestPacket : AbstractPacket
{
    public readonly Guid uuid;
    public readonly string reason;
    public readonly int difference;
    public override PacketType PacketType
    {
        get { return PacketType.ClientTransaction; }
    }

    public BalanceSetupRequestPacket(Guid uuid, string reason, int difference)
        : base(DateTime.Now.Millisecond & 0xfffffff)
    {
        (this.uuid, this.reason, this.difference) = (uuid, reason, difference);
    }

    protected override JToken? ToJsonData()
    {
        return new JObject()
        {
            ["reason"] = reason,
            ["player"] = uuid.ToString("N"),
            ["amount"] = difference
        };
    }
}