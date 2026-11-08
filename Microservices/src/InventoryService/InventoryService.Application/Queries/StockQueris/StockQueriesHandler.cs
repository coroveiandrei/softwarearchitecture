using MediatR;
using InventoryService.Application.RemoteServices;
using Newtonsoft.Json;
using InventoryService.Application.Common;
using InventoryService.Domain.Aggregates;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Events;
using InventoryService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.Application.Queries.StockQueris
{
    public class StockQueriesHandler : IRequestHandler<GetStockByIdQuery, GetStockByIdQueryResponse>,
                                    IRequestHandler<GetAllStocksQuery, GetAllStocksQueryResponse>
    {

        private readonly IStockProjectionRepository _stocksRepository;
        private readonly IPlacementService _placementService;
        public StockQueriesHandler(IStockProjectionRepository stocksRepository, IPlacementService placementService)
        {
            _stocksRepository = stocksRepository;
            _placementService = placementService;
        }

        public async Task<GetStockByIdQueryResponse> Handle(GetStockByIdQuery request, CancellationToken cancellationToken)
        {
            var stockFromTable = _stocksRepository.GetStockById(request.Id);
            if (stockFromTable != null)
            {
                var locations = await _placementService.GetReservedLocations();
                stockFromTable.Location = locations.TryGetValue(stockFromTable.LicensePlate, out var location) ? location : null;
            }
            return new GetStockByIdQueryResponse()
            {
                Stock = stockFromTable
            };
        }

        public async Task<GetAllStocksQueryResponse> Handle(GetAllStocksQuery request, CancellationToken cancellationToken)
        {
            var stocks = _stocksRepository.GetAllStocks().ToList();
            if (stocks.Count > 0)
            {
                var locations = await _placementService.GetReservedLocations();
                foreach (var stock in stocks)
                {
                    stock.Location = locations.TryGetValue(stock.LicensePlate, out var location) ? location : null;
                }
            }

            return new GetAllStocksQueryResponse()
            {
                Stocks = stocks
            };
        }
    }
}
