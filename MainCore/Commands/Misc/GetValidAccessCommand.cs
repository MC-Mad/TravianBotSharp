using System.Collections.Concurrent;
using System.Net;

namespace MainCore.Commands.Misc
{
    [Handler]
    public static partial class GetValidAccessCommand
    {
        public sealed record Command(AccountId AccountId, bool IgnoreSleepTime = false) : IAccountCommand;

        private static async ValueTask<Result<AccessDto>> HandleAsync(
            Command command,
            ILogger logger,
            AppDbContext context
            )
        {
            var (accountId, ignoreSleepTime) = command;

            var accesses = context.Accesses
               .Where(x => x.AccountId == accountId.Value)
               .OrderBy(x => x.LastUsed) // get oldest one
               .ToDto()
               .ToList();

            async Task<AccessDto?> GetValidAccess(List<AccessDto> proxies)
            {
                foreach (var proxy in proxies)
                {
                    var client = GetHttpClient(proxy);
                    logger.Information("Checking proxy {Proxy}, last used {LastUsed}", proxy.Proxy, proxy.LastUsed);
                    try
                    {
                        var response = await client.GetAsync(TRAVIAN_PAGE);
                        if (response.IsSuccessStatusCode)
                        {
                            logger.Information("Access {Proxy} is good", proxy.Proxy);
                            return proxy;
                        }

                        logger.Warning("Access {Proxy} is not working, status code: {StatusCode}", proxy.Proxy, response.StatusCode);
                        continue;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "{Message}", ex.Message);
                    }
                }
                return null;
            }

            var access = await GetValidAccess(accesses);
            if (access is null) return Stop.AllAccessNotWorking;

            if (accesses.Count == 1) return access;
            if (ignoreSleepTime) return access;

            var minSleep = context.ByName(accountId, AccountSettingEnums.SleepTimeMin);
            var timeValid = DateTime.Now.AddMinutes(-minSleep);
            if (access.LastUsed > timeValid) return Stop.LackOfAccess;
            return access;
        }

        private static readonly HttpClient _defaultHttpClient = new(new HttpClientHandler()
        {
            UseProxy = false,
        });

        private static readonly ConcurrentDictionary<string, HttpClient> _proxyHttpClients = new();

        private const string TRAVIAN_PAGE = "https://www.travian.com/international";

        private static HttpClient GetHttpClient(AccessDto access)
        {
            if (string.IsNullOrEmpty(access.ProxyHost)) return _defaultHttpClient;

            var key = $"{access.ProxyHost}:{access.ProxyPort}|{access.ProxyUsername}|{access.ProxyPassword}";
            return _proxyHttpClients.GetOrAdd(key, _ => CreateProxyHttpClient(access));
        }

        private static HttpClient CreateProxyHttpClient(AccessDto access)
        {
            var proxy = new WebProxy($"http://{access.ProxyHost}:{access.ProxyPort}");
            if (!string.IsNullOrEmpty(access.ProxyUsername))
            {
                proxy.Credentials = new NetworkCredential(access.ProxyUsername, access.ProxyPassword);
            }

            return new HttpClient(new HttpClientHandler()
            {
                Proxy = proxy,
                UseProxy = true,
            });
        }
    }
}