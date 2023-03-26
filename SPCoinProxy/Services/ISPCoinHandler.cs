namespace SPCoinProxy.Services;

public interface ISPCoinHandler
{
    Task<bool> IncreaseUserBalance(Guid uuid, string reason, int value, CancellationToken cancellationToken);
    Task<bool> DecreaseUserBalance(Guid uuid, string reason, int value, CancellationToken cancellationToken);
    Task<int> GetUserBalance(Guid uuid, CancellationToken cancellationToken);
}