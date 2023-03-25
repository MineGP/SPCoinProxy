namespace SPCoinProxy.Services
{
    public interface IProxyContext
    {
        (string url, string token) GetContext();
    }
}