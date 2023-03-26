using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPCoinTest.Packets;

public enum SendType
{
    Client,
    Server
}
public enum PacketType
{
    ClientAuth,
    ClientConfirm,
    ClientBalance,
    ClientTransaction,
    ServerBan,
    ServerWhitelist,
    ServerCommand,
    ServerSendToken,
    ServerBalance,
    ServerConfirm,
    ServerBad
}
public static class PacketTypeExt
{
    public static string GetKey(this PacketType type)
    {
        return type.ToString()[6..].ToLower();
    }

    public static SendType GetSendType(this PacketType type)
    {
        return type.ToString().StartsWith("Client") ? SendType.Client : SendType.Server;
    }
}
public abstract class AbstractPacket
{
    [JsonIgnore] public abstract PacketType PacketType { get; }
    [JsonIgnore] public long id;

    public AbstractPacket(long id)
    {
        this.id = id;
    }

    public AbstractPacket(long id, JObject data)
    {
        this.id = id;
    }

    protected virtual JToken? ToJsonData()
    {
        var data = JObject.FromObject(this);
        return data.Count == 0 ? null : data;
    }

    public JObject ToJson()
    {
        var json = new JObject()
        {
            ["type"] = PacketType.GetKey()
        };
        if (id != 0) json["id"] = id;
        if (ToJsonData() is JToken data) json["data"] = data;
        return json;
    }
}