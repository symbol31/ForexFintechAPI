using ForexFintechAPI.Data;
using ForexFintechAPI.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Net;
using System.Text;

public class RateLimitingMiddleware : IMiddleware
{
    private readonly IDistributedCache _cache;
    private readonly MyContext _myContext;

    public RateLimitingMiddleware( IDistributedCache cache, MyContext myContext)
    {
        _cache = cache;
        _myContext = myContext;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.GetEndpoint();
        var rateLimitingDecorator = endpoint?.Metadata.GetMetadata<Limit>();

        if (rateLimitingDecorator is null)
        {
            await next(context);
            return;
        }

        var key = GenerateClientKey(context);
        var clientStatistics = await GetClientStatisticsByKey(key);
        var limit = GetLimitingData();

        if (clientStatistics != null 
            && DateTime.UtcNow < clientStatistics.LastSuccessfulResponseTime.AddSeconds(limit.TimeWindow) 
            && clientStatistics.NumberOfRequestsCompletedSuccessfully == limit.MaxRequests)
        {
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            return;
        }

        await UpdateClientStatisticsStorage(key, limit.MaxRequests);
        await next(context);
    }

    private static string GenerateClientKey(HttpContext context) => $"{context.Request.Path}_{context.Connection.RemoteIpAddress}";

    private async Task<ClientStatistics> GetClientStatisticsByKey(string key)
    {
        var cachedData = await _cache.GetAsync(key);

        if (cachedData != null)
        {
            return JsonConvert.DeserializeObject<ClientStatistics>(Encoding.UTF8.GetString(cachedData));
        }

        return null;
    }

    private async Task UpdateClientStatisticsStorage(string key, int maxRequests)
    {
        var clientStat = await GetClientStatisticsByKey(key);

        if (clientStat != null)
        {
            clientStat.LastSuccessfulResponseTime = DateTime.UtcNow;

            if (clientStat.NumberOfRequestsCompletedSuccessfully == maxRequests)
            {
                clientStat.NumberOfRequestsCompletedSuccessfully = 1;
            }
            else
            {
                clientStat.NumberOfRequestsCompletedSuccessfully++;
            }

            var serializedData = JsonConvert.SerializeObject(clientStat);
            await _cache.SetAsync(key, Encoding.UTF8.GetBytes(serializedData));
        }
        else
        {
            var clientStatistics = new ClientStatistics
            {
                LastSuccessfulResponseTime = DateTime.UtcNow,
                NumberOfRequestsCompletedSuccessfully = 1
            };

            var serializedData = JsonConvert.SerializeObject(clientStatistics);
            await _cache.SetAsync(key, Encoding.UTF8.GetBytes(serializedData));
        }
    }

    private LimitingData GetLimitingData()
    {
        var result = _myContext.LimitingData.FirstOrDefault();
        var LimitingData = new LimitingData
        {
            TimeWindow = result.TimeWindow,
            MaxRequests = result.MaxRequests
        };
        return LimitingData;
    }

}
public class ClientStatistics
{
    public DateTime LastSuccessfulResponseTime { get; set; }
    public int NumberOfRequestsCompletedSuccessfully { get; set; }
}