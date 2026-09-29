using System;
using System.Threading;

namespace PlacementService.FailureSimulation;

public sealed class ReservationFailureSimulator
{
    public const string EnvironmentVariable = "PLACEMENT_FAILURE_COUNT";

    private readonly bool _alwaysFail;
    private int _remainingFailures;

    public ReservationFailureSimulator(int failureCount)
    {
        _alwaysFail = failureCount < 0;
        _remainingFailures = Math.Max(0, failureCount);
    }

    public static ReservationFailureSimulator FromEnvironment()
    {
        var configuredValue = Environment.GetEnvironmentVariable(EnvironmentVariable);
        return new ReservationFailureSimulator(
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
