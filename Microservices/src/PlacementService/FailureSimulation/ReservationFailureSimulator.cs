using System;
using System.Collections.Concurrent;

namespace PlacementService.FailureSimulation;

public sealed class ReservationFailureSimulator
{
    public const string EnvironmentVariable = "PLACEMENT_FAILURE_COUNT";

    private readonly bool _alwaysFail;
    private readonly int _failureCount;
    private readonly ConcurrentDictionary<string, int> _remainingFailures = new();

    public ReservationFailureSimulator(int failureCount)
    {
        _alwaysFail = failureCount < 0;
        _failureCount = Math.Max(0, failureCount);
    }

    public static ReservationFailureSimulator FromEnvironment()
    {
        var configuredValue = Environment.GetEnvironmentVariable(EnvironmentVariable);
        return new ReservationFailureSimulator(
            int.TryParse(configuredValue, out var failureCount) ? failureCount : 0);
    }

    public bool ShouldFail(string objectId)
    {
        if (_alwaysFail)
        {
            return true;
        }

        if (_failureCount == 0)
        {
            return false;
        }

        while (true)
        {
            var currentValue = _remainingFailures.GetOrAdd(objectId, _failureCount);
            if (currentValue == 0)
            {
                return false;
            }

            if (_remainingFailures.TryUpdate(objectId, currentValue - 1, currentValue))
            {
                return true;
            }
        }
    }
}
