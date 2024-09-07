using InventoryService.Domain.Models;
using InventoryService.Domain.Projections;
using System;
using System.Collections.Generic;

namespace InventoryService.Domain.Repositories
{
    public interface IStockProjectionRepository
    {
        public IEnumerable<StockProjection> GetAllStocks();
        public StockProjection GetStockById(Guid id);
        public StockProjection GetStockByLicensePlate(string licensePlate);
        public void CreateStock(StockProjection stock);
        public void UpdateStock(Guid id, StockModel model, int version);
        public void DeleteStock(Guid id);
    }
}
