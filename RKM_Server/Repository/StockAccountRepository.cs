using RKM_Server.Data;
using RKM_Server.DTO;
using RKM_Server.Interfaces;
using RKM_Server.Models;
using System.Text;
using System.Security.Cryptography;

namespace RKM_Server.Repository
{
    public class StockAccountRepository : StockAccountInterface
    {
        private readonly AppDbContext _context;
        public StockAccountRepository(AppDbContext context)
        {
            _context = context;
        }
        public bool CreateStockAccount(StockAccount stockAccount)
        {
            _context.Add(stockAccount);
            return Save();
        }

        public bool DeleteStockAccount(StockAccount stockAccount)
        {
            _context.Remove(stockAccount);
            return Save();
        }

        public StockAccount GetStockAccount(int id)
        {
            return _context.StockAccounts.Where(so => so.Id == id).FirstOrDefault();
        }

        public ICollection<StockAccount> GetStockAccounts()
        {
            return _context.StockAccounts.OrderBy(o => o.Id).ToList();
        }

        public StockAccount GetStockAccountTrimToUpper(StockAccountDto stockAccountCreate)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool StockAccountExist(int id)
        {
            return _context.StockAccounts.Any(p => p.Id == id);
        }

        public bool UpdateStockAccount(StockAccount stockAccount)
        {
            _context.Update(stockAccount);
            return Save();
        }
    }
}
