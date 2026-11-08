using System.Net;
using System.Text;
using InventoryService.Application.Queries.StockQueris;
using InventoryService.Domain.Models;
using InventoryService.Domain.Projections;
using InventoryService.Domain.Repositories;
using Xunit;

namespace Microservices.WorkshopTests;

public class StockLocationTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Stock_get_uses_placement_reservation_instead_of_stored_input(bool reserved)
    {
        var stock = new StockProjection { Id = Guid.NewGuid(), LicensePlate = "STOCK-1", Location = "string" };
        using var client = new HttpClient(new ReservationHandler(reserved)) { BaseAddress = new Uri("http://placement/") };
        var placement = new InventoryService.Infrastructure.RemoteServices.PlacementService(client);
        var handler = new StockQueriesHandler(new StockRepository(stock), placement);

        var result = await handler.Handle(new GetStockByIdQuery { Id = stock.Id }, CancellationToken.None);

        Assert.Equal(reserved ? "A-1" : null, result.Stock.Location);
    }

    [Fact]
    public async Task Stock_list_uses_current_reservations()
    {
        var stock = new StockProjection { Id = Guid.NewGuid(), LicensePlate = "STOCK-1", Location = "string" };
        using var client = new HttpClient(new ReservationHandler(true)) { BaseAddress = new Uri("http://placement/") };
        var placement = new InventoryService.Infrastructure.RemoteServices.PlacementService(client);
        var handler = new StockQueriesHandler(new StockRepository(stock), placement);

        var result = await handler.Handle(new GetAllStocksQuery(), CancellationToken.None);

        Assert.Equal("A-1", Assert.Single(result.Stocks).Location);
    }

    private sealed class ReservationHandler(bool reserved) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Assert.Equal("/GetReservedLocations", request.RequestUri!.AbsolutePath);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(reserved ? "{\"STOCK-1\":\"A-1\"}" : "{}", Encoding.UTF8, "application/json")
            });
        }
    }

    private sealed class StockRepository(StockProjection stock) : IStockProjectionRepository
    {
        public IEnumerable<StockProjection> GetAllStocks() => new[] { stock };
        public StockProjection GetStockById(Guid id) => stock;
        public StockProjection GetStockByLicensePlate(string licensePlate) => stock;
        public void CreateStock(StockProjection value) => throw new NotSupportedException();
        public void UpdateStock(Guid id, StockModel model, int version) => throw new NotSupportedException();
        public void DeleteStock(Guid id) => throw new NotSupportedException();
    }
}
