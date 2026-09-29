using System;
using System.Threading;

namespace TaskManagement.WebUI.FailureSimulation;

public sealed class PlacementEventFailureSimulator
{
    public const string EnvironmentVariable = "PLACEMENT_EVENT_FAILURE_COUNT";

    private readonly bool _alwaysFail;
    private int _remainingFailures;

    public PlacementEventFailureSimulator(int failureCount)
    {
        _alwaysFail = failureCount < 0;
        _remainingFailures = Math.Max(0, failureCount);
    }

    public static PlacementEventFailureSimulator FromEnvironment()
    {
        var configuredValue = Environment.GetEnvironmentVariable(EnvironmentVariable);
        return new PlacementEventFailureSimulator(
            int.TryParse(configuredValue, out var failureCount) ? failureCount : 0);
    }

    public bool ShouldFail()
    {
        if (_alwaysFail)
        {
            return true;
        }

        while (true)
        {
            var currentValue = Volatile.Read(ref _remainingFailures);
            if (currentValue == 0)
            {
                return false;
            }

            if (Interlocked.CompareExchange(
                    ref _remainingFailures,
                    currentValue - 1,
                    currentValue) == currentValue)
            {
                return true;
            }
        }
    }
}
