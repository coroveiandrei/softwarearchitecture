using MediatR;
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
        public StockQueriesHandler(IStockProjectionRepository stocksRepository)
        {
            _stocksRepository = stocksRepository;
        }

        public async Task<GetStockByIdQueryResponse> Handle(GetStockByIdQuery request, CancellationToken cancellationToken)
        {
            var stockFromTable = _stocksRepository.GetStockById(request.Id);
            return new GetStockByIdQueryResponse()
            {
                Stock = stockFromTable
            };
        }

        public async Task<GetAllStocksQueryResponse> Handle(GetAllStocksQuery request, CancellationToken cancellationToken)
        {
            var stocks = _stocksRepository.GetAllStocks();

            return new GetAllStocksQueryResponse()
            {
                Stocks = stocks
            };
        }
    }
}
