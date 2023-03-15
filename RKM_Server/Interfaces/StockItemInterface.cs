using RKM_Server.DTO;
using RKM_Server.Models;

namespace RKM_Server.Interfaces
{
    public interface StockItemInterface
    {
        ICollection<StockItem> GetStockItems();
        StockItem GetStockItem(string itemname);
        StockItem GetStockItem(int id);
        bool StockItemExist(string itemname);
        bool StockItemExist(int id);
        bool CreateStockItem(StockItem stockItem);
        bool UpdateStockItem(StockItem stockItem);
        StockItem GetStockItemTrimToUpper(StockItemDto stockitemCreate);
        bool DeleteStockItem(StockItem stockItem);
        bool Save();



    }
}
