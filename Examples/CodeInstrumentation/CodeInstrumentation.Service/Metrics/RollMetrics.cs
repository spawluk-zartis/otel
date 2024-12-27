using System.Diagnostics.Metrics;

namespace CodeInstrumentation.Service.Metrics;

public class RollMetrics
{
    public static string Name = "RollMetrics";

    private readonly Histogram<int> _rollResultHistogram;

    public RollMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(Name);

        _rollResultHistogram = meter.CreateHistogram<int>("roll.results");
    }

    public void RecordResult(int result) => _rollResultHistogram.Record(result);
}
