using RKM_Server.DTO;
using RKM_Server.Models;

namespace RKM_Server.Interfaces
{
    public interface OrdererInterface
    {
        ICollection<Orderer> GetOrderers();
        Orderer GetOrderer(int id);
        bool OrdererExist(int id);
        bool CreateOrderer(Orderer orderer);
        bool UpdateOrderer(Orderer orderer);
        Orderer GetOrdererTrimToUpper(OrdererDto ordererCreate);
        bool DeleteOrderer(Orderer orderer);
        bool Save();
    }
}
