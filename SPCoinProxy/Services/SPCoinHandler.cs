using Newtonsoft.Json.Linq;
using SPCoinTest.Packets;
using System.Net.WebSockets;
using System.Text;

namespace SPCoinProxy.Services;

public class SPCoinHandler : ISPCoinHandler
{
    private readonly string url;
    private readonly string token;
    public SPCoinHandler(IProxyContext context)
        => (url, token) = context.GetContext();

    public Task<int> GetUserBalance(Guid uuid, CancellationToken cancellationToken) => RunClient(async s =>
    {
        var request = new BalanceRequestPacket(uuid);
        await s.WriteAsync(request);
        var response = await s.ReadAsync<BalanceResponsePacket>();
        return response.balance;
    }, cancellationToken);
    public Task<bool> IncreaseUserBalance(Guid uuid, string reason, int value, CancellationToken cancellationToken) => RunClient(async s =>
    {
        if (value <= 0) return false;
        var request = new BalanceSetupRequestPacket(uuid, reason, value);
        await s.WriteAsync(request);
        try
        {
            await s.ReadAsync<ConfirmResponsePacket>().WaitAsync(TimeSpan.FromSeconds(3));
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }, cancellationToken);
    public Task<bool> DecreaseUserBalance(Guid uuid, string reason, int value, CancellationToken cancellationToken) => RunClient(async s =>
    {
        if (value <= 0) return false;
        var request = new BalanceSetupRequestPacket(uuid, reason, -value);
        await s.WriteAsync(request);
        try
        {
            await s.ReadAsync<ConfirmResponsePacket>().WaitAsync(TimeSpan.FromSeconds(3));
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }, cancellationToken);

    private async Task RunClient(Func<SPCoinSocket, Task> task, CancellationToken cancellationToken)
    {
        using var socket = new SPCoinSocket(url, cancellationToken);
        await socket.ConnectAsync();
        await socket.WriteAsync(new LoginRequestPacket(token));
        await task.Invoke(socket);
    }
    private async Task<T> RunClient<T>(Func<SPCoinSocket, Task<T>> task, CancellationToken cancellationToken)
    {
        using var socket = new SPCoinSocket(url, cancellationToken);
        await socket.ConnectAsync();
        await socket.WriteAsync(new LoginRequestPacket(token));
        return await task.Invoke(socket);
    }

    private class SPCoinSocket : IDisposable
    {
        private readonly ClientWebSocket socket;
        private readonly string url;
        private readonly CancellationToken cancellationToken;

        public SPCoinSocket(string url, CancellationToken cancellationToken)
        {
            socket = new ClientWebSocket();
            this.url = url;
            this.cancellationToken = cancellationToken;
        }

        public Task ConnectAsync() => socket.ConnectAsync(new Uri(url), cancellationToken);

        public async Task<JObject> ReadJsonAsync()
            => JObject.Parse(await ReadStringAsync(socket, cancellationToken));

        public async Task<T> ReadAsync<T>() where T : AbstractPacket
        {
            T packet;
            var json = await ReadJsonAsync();

            var _type = json["type"]?.Value<string>();
            string _key;

            if (json["data"] is JValue jvalue && jvalue?.Value<long>() is long _id)
            {
                packet = (T)Activator.CreateInstance(typeof(T), _id, new JObject())!;
                _key = packet.PacketType.GetKey();
                if (_key != _type) throw new Exception($"Type of packet {json.ToString(Newtonsoft.Json.Formatting.None)} is not equals: {_key} != {_type ?? "NULLABLE"}");
                return packet;
            }
            if (json["data"] is not JObject data || data["reqId"]?.Value<long>() is not long id) throw new Exception($"Id of packet {json.ToString(Newtonsoft.Json.Formatting.None)} is empty");
            packet = (T)Activator.CreateInstance(typeof(T), id, data)!;
            _key = packet.PacketType.GetKey();
            if (_key != _type) throw new Exception($"Type of packet {json.ToString(Newtonsoft.Json.Formatting.None)} is not equals: {_key} != {_type ?? "NULLABLE"}");
            return packet;
        }
        public Task WriteAsync<T>(T packet) where T : AbstractPacket
            => WriteStringAsync(socket, packet.ToJson().ToString(Newtonsoft.Json.Formatting.None), cancellationToken);

        public void Dispose() => socket.Dispose();
    }

    private static async Task<string> ReadStringAsync(WebSocket socket, CancellationToken cancellationToken)
    {
        var buffer = WebSocket.CreateClientBuffer(1024, 1024);
        WebSocketReceiveResult taskResult;

        var result = new StringBuilder();

        do
        {
            taskResult = await socket.ReceiveAsync(buffer, cancellationToken);
            result.Append(Encoding.UTF8.GetString(buffer));
        } while (!taskResult.EndOfMessage);

        return result.ToString();
    }
    private static Task WriteStringAsync(WebSocket socket, string data, CancellationToken cancellationToken)
        => socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, cancellationToken);
}