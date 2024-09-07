using InventoryService.Application.Common;
using InventoryService.Domain.Aggregates;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Projections;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Application.Queries.StockQueris
{
    public class GetAllStocksQuery : BaseRequest<GetAllStocksQueryResponse>
    {
    }

    public class GetAllStocksQueryResponse
    {
        public IEnumerable<StockProjection> Stocks { get; set; }
    }

    public class GetStockByIdQuery : BaseRequest<GetStockByIdQueryResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetStockByIdQueryResponse
    {
        public StockProjection Stock { get; set; }
    }

}
