using RKM_Server.Data;
using RKM_Server.DTO;
using RKM_Server.Interfaces;
using RKM_Server.Models;
using System.Text;
using System.Security.Cryptography;

namespace RKM_Server.Repository
{
    public class StockItemRepository : StockItemInterface
    {
        private readonly AppDbContext _context;
        public StockItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool CreateStockItem(StockItem stockItem)
        {
            _context.Add(stockItem);
            return Save();
        }

        public bool DeleteStockItem(StockItem stockItem)
        {
            _context.Remove(stockItem);
            return Save();
        }

        public StockItem GetStockItem(string itemname)
        {
            return _context.StockItems.Where(s => s.ItemType == itemname).FirstOrDefault();
        }

        public StockItem GetStockItem(int id)
        {
            return _context.StockItems.Where(s => s.Id == id).FirstOrDefault();
        }

        public ICollection<StockItem> GetStockItems()
        {
            return _context.StockItems.OrderBy(s => s.Id).ToList();
        }

        public StockItem GetStockItemTrimToUpper(StockItemDto stockitemCreate)
        {
            return GetStockItems().Where(x => x.ItemType.Trim().ToUpper() == stockitemCreate.ItemType.TrimEnd().ToUpper())
                .FirstOrDefault();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool StockItemExist(string itemname)
        {
            return _context.StockItems.Any(s => s.ItemType == itemname);
        }

        public bool StockItemExist(int id)
        {
            return _context.StockItems.Any(s => s.Id == id);
        }

        public bool UpdateStockItem(StockItem stockItem)
        {
            _context.Update(stockItem);
            return Save();
        }
    }
}
