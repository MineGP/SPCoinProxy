using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoinTest.Packets;

public class BalanceResponsePacket : AbstractPacket
{
    public readonly int balance;
    public override PacketType PacketType
    {
        get { return PacketType.ServerBalance; }
    }

    public BalanceResponsePacket(long id, JObject json)
        : base(id, json)
    {
        this.balance = json["balance"]!.Value<int>();
    }
}