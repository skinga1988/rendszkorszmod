using RKM_Server.DTO;
using RKM_Server.Models;

namespace RKM_Server.Interfaces
{
    public interface StockAccountInterface
    {
        ICollection<StockAccount> GetStockAccounts();
        StockAccount GetStockAccount(int id);
        bool StockAccountExist(int id);
        bool CreateStockAccount(StockAccount stockAccount);
        bool UpdateStockAccount(StockAccount stockAccount);
        StockAccount GetStockAccountTrimToUpper(StockAccountDto stockAccountCreate);
        bool DeleteStockAccount(StockAccount stockAccount);
        bool Save();
    }
}
