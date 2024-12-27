using System.Text.Json;
using AspireShop.BasketService.Metrics;
using StackExchange.Redis;
using AspireShop.BasketService.Models;

namespace AspireShop.BasketService.Repositories;

public class RedisBasketRepository(ILogger<RedisBasketRepository> logger, IConnectionMultiplexer redis, IBasketServiceMetrics metrics) : IBasketRepository
{
    private readonly IDatabase _database = redis.GetDatabase();
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task<bool> DeleteBasketAsync(string id)
    {
        metrics.BasketRemoved(id);
        return await _database.KeyDeleteAsync(id);
    }

    public IEnumerable<string> GetUsers()
    {
        var server = GetServer();
        var data = server.Keys();

        return data?.Select(k => k.ToString()) ?? [];
    }

    public async Task<CustomerBasket?> FindBasketAsync(string customerId)
    {
        var data = await _database.StringGetAsync(customerId);

        if (data.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<CustomerBasket>(data!, _jsonSerializerOptions);
    }

    public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
    {
        if (basket.BuyerId == null)
        {
            return null;
        }

        var created = await _database.StringSetAsync(basket.BuyerId, JsonSerializer.Serialize(basket, _jsonSerializerOptions));

        if (!created)
        {
            logger.LogInformation("Problem occur persisting the item.");
            return null;
        }

        logger.LogInformation("Basket item persisted successfully.");

        return await FindBasketAsync(basket.BuyerId);
    }

    private IServer GetServer()
    {
        var endpoint = redis.GetEndPoints();
        return redis.GetServer(endpoint.First());
    }
}
