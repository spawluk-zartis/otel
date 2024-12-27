using System.Diagnostics;

namespace CodeInstrumentation.Service.Extensions;

internal static class ActivityExtensions
{
    internal static void RecordRoll(this Activity activity, int rollResult) => activity.SetTag("roll.result", rollResult);
}
