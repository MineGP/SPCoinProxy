namespace SPCoinProxy.Services
{
    public class ProxyContext : IProxyContext
    {
        private readonly IHttpContextAccessor _accessor;
        public ProxyContext(IHttpContextAccessor accessor) => _accessor = accessor;

        public (string url, string token) GetContext()
            => (((string?)_accessor.HttpContext?.Request?.Headers["ws"])!, ((string?)_accessor.HttpContext?.Request?.Headers["token"])!);
    }
}
