using Newtonsoft.Json.Linq;

namespace SPCoinTest.Packets
{
    public class LoginRequestPacket : AbstractPacket
    {
        public readonly string token;
        public override PacketType PacketType => PacketType.ClientAuth;

        public LoginRequestPacket(string token) : base(-1) => this.token = token;
        protected override JToken? ToJsonData() => token;
    }
}
