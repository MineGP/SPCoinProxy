using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoinTest.Packets;

public class ConfirmResponsePacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.ServerConfirm;

    public ConfirmResponsePacket(long id, JObject json) : base(id, json) { }
}