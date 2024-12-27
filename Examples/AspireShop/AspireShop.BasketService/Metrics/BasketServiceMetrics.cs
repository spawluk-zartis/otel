using System.Diagnostics.Metrics;

namespace AspireShop.BasketService.Metrics;

public interface IBasketServiceMetrics
{
    void BasketCreated(string buyerId);

    void BasketRemoved(string buyerId);
}

internal sealed class BasketServiceMetrics : IBasketServiceMetrics
{
    internal static readonly string MeterName = typeof(BasketServiceMetrics).FullName!;
    private readonly UpDownCounter<long> _basketsCounter;

    public BasketServiceMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(typeof(BasketServiceMetrics).FullName!);

        _basketsCounter = meter.CreateUpDownCounter<long>("baskets.count");
    }

    public void BasketCreated(string buyerId)
    {
        _basketsCounter.Add(1, new KeyValuePair<string, object?>("buyer.id", buyerId));
    }

    public void BasketRemoved(string buyerId)
    {
        _basketsCounter.Add(-1, new KeyValuePair<string, object?>("buyer.id", buyerId));
    }
}
