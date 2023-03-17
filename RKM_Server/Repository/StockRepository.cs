using RKM_Server.Data;
using RKM_Server.DTO;
using RKM_Server.Interfaces;
using RKM_Server.Models;
using System.Text;
using System.Security.Cryptography;

namespace RKM_Server.Repository
{
    public class StockRepository : StockInterface
    {
        private readonly AppDbContext _context;
        public StockRepository(AppDbContext context)
        {
            _context = context;
        }
        public bool CreateStock(Stock stock)
        {
            _context.Add(stock);
            return Save();
        }

        public bool DeleteStock(Stock stock)
        {
            _context.Remove(stock);
            return Save();
        }

        public Stock GetStock(int id)
        {
            return _context.Stocks.Where(s => s.Id == id).FirstOrDefault();
        }

        public ICollection<Stock> GetStocks()
        {
            return _context.Stocks.OrderBy(s => s.Id).ToList();
        }

        public Stock GetStockTrimToUpper(StockDto stockCreate)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool StockExist(int id)
        {
            return _context.Stocks.Any(s => s.Id == id);
        }

        public bool UpdateStock(Stock stock)
        {
            _context.Update(stock);
            return Save();
        }
    }
}
