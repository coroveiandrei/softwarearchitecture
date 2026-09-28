using DDDCqrsEs.Application.Common;
using DDDCqrsEs.Common;
using DDDCqrsEs.Common.Constants;
using DDDCqrsEs.Domain.Entities;
using DDDCqrsEs.Domain.Models;
using DDDCqrsEs.Domain.Projections;
using DDDCqrsEs.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DDDCqrsEs.Persistance.Repositories
{
    [MapServiceDependency(Name: nameof(StockProjectionRepository))]
    public class StockProjectionRepository : IStockProjectionRepository
    {
        private ToDoDbContext dbContext;
        public StockProjectionRepository(ToDoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<StockProjection> GetAllStocks()
        {

            return dbContext.Stocks.ToList();
        }

        public StockProjection GetStockById(Guid id)
        {
            var stock = dbContext.Stocks.FirstOrDefault(s => s.Id == id);
            if (stock != null)
            {
                stock.BestBeforeDate = stock.BestBeforeDate.ToLocalTime();
            }
            return stock;
        }

        public void CreateStock(StockProjection stock)
        {
            var existingStock = dbContext.Stocks.FirstOrDefault(s => s.Id == stock.Id);
            if (existingStock != null)
            {
                return;
            }

            dbContext.Add(stock);
            dbContext.SaveChanges();
        }

        public void DeleteStock(Guid stockId, int version)
        {

            var stockToBeDeleted = dbContext.Stocks.FirstOrDefault(s => s.Id == stockId);
            if (stockToBeDeleted != null && stockToBeDeleted.Version < version)
            {
                stockToBeDeleted.Status = StockStatusValues.CLOSED;
                stockToBeDeleted.Version = version;
                dbContext.SaveChanges();
            }

        }

        public void UpdateStock(Guid stockId, StockModel stock, int version)
        {

            var stockToBeUpdated = dbContext.Stocks.FirstOrDefault(s => s.Id == stockId);
            if (stockToBeUpdated != null && stockToBeUpdated.Version < version)
            {
                ModelMapper.MapModelIntoProjection(stockToBeUpdated, stock);
                stockToBeUpdated.Version = version;
                dbContext.SaveChanges();
            }

        }

        public StockProjection GetStockByLicensePlate(string licensePlate)
        {

            var stock = dbContext.Stocks.FirstOrDefault(s => s.LicensePlate == licensePlate);
            return stock;

        }
    }
}
