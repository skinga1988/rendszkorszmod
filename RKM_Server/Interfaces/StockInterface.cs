using RKM_Server.DTO;
using RKM_Server.Models;

namespace RKM_Server.Interfaces
{
    public interface StockInterface
    {
        ICollection<Stock> GetStocks();
        Stock GetStock(int id);
        bool StockExist(int id);
        bool CreateStock(Stock stock);
        bool UpdateStock(Stock stock);
        Stock GetStockTrimToUpper(StockDto stockCreate);
        bool DeleteStock(Stock stock);
        bool Save();
    }
}
