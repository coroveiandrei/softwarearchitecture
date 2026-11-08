using PlacementService.FailureSimulation;
using TaskManagement.WebUI.FailureSimulation;
using Xunit;
using System.Text.Json;

namespace Microservices.WorkshopTests;

public class FailureSimulatorTests
{
    [Fact]
    public void ReservationSimulatorFailsConfiguredNumberOfTimes()
    {
        var simulator = new ReservationFailureSimulator(2);

        Assert.True(simulator.ShouldFail("stock-1"));
        Assert.True(simulator.ShouldFail("stock-1"));
        Assert.False(simulator.ShouldFail("stock-1"));
        Assert.False(simulator.ShouldFail("stock-1"));
    }

    [Fact]
    public void ReservationFailuresAreIndependentForEachStock()
    {
        var simulator = new ReservationFailureSimulator(2);

        Assert.True(simulator.ShouldFail("stock-1"));
        Assert.True(simulator.ShouldFail("stock-2"));
        Assert.True(simulator.ShouldFail("stock-1"));
        Assert.False(simulator.ShouldFail("stock-1"));
        Assert.True(simulator.ShouldFail("stock-2"));
        Assert.False(simulator.ShouldFail("stock-2"));
    }

    [Fact]
    public void ConcurrentReservationAttemptsFailExactlyTheConfiguredNumberOfTimes()
    {
        var simulator = new ReservationFailureSimulator(2);
        var failures = 0;

        Parallel.For(0, 20, _ =>
        {
            if (simulator.ShouldFail("stock-1"))
            {
                Interlocked.Increment(ref failures);
            }
        });

        Assert.Equal(2, failures);
    }

    [Fact]
    public void PlacementEventSimulatorFailsConfiguredNumberOfTimes()
    {
        var simulator = new PlacementEventFailureSimulator(2);

        Assert.True(simulator.ShouldFail());
        Assert.True(simulator.ShouldFail());
        Assert.False(simulator.ShouldFail());
        Assert.False(simulator.ShouldFail());
    }

    [Fact]
    public void NegativeFailureCountAlwaysFails()
    {
        var reservationSimulator = new ReservationFailureSimulator(-1);
        var eventSimulator = new PlacementEventFailureSimulator(-1);

        Assert.True(reservationSimulator.ShouldFail("stock-1"));
        Assert.True(reservationSimulator.ShouldFail("stock-1"));
        Assert.True(eventSimulator.ShouldFail());
        Assert.True(eventSimulator.ShouldFail());
    }

    [Fact]
    public void PlacementEventContractPreservesItsIdentity()
    {
        var eventId = Guid.NewGuid();
        var publishedEvent = new PlacementService.Events.PlacementReservationCreated(
            eventId,
            "vehicle-42",
            "A-1");

        var json = JsonSerializer.Serialize(publishedEvent);
        var receivedEvent = JsonSerializer.Deserialize<
            TaskManagement.Application.Remote.PlacementReservationCreated>(json);

        Assert.NotNull(receivedEvent);
        Assert.Equal(eventId, receivedEvent.EventId);
        Assert.Equal("vehicle-42", receivedEvent.ObjectId);
        Assert.Equal("A-1", receivedEvent.Location);
    }
}
