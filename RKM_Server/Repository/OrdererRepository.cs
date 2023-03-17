using RKM_Server.Data;
using RKM_Server.DTO;
using RKM_Server.Interfaces;
using RKM_Server.Models;
using System.Text;
using System.Security.Cryptography;

namespace RKM_Server.Repository
{
    public class OrdererRepository : OrdererInterface
    {
        private readonly AppDbContext _context;
        public OrdererRepository(AppDbContext context)
        {
            _context = context;
        }
        public bool CreateOrderer(Orderer orderer)
        {
            _context.Add(orderer);
            return Save();
        }

        public bool DeleteOrderer(Orderer orderer)
        {
            _context.Remove(orderer);
            return Save();
        }

        public Orderer GetOrderer(int id)
        {
            return _context.Orderers.Where(o => o.Id == id).FirstOrDefault();
        }

        public ICollection<Orderer> GetOrderers()
        {
            return _context.Orderers.OrderBy(o => o.Id).ToList();
        }

        public Orderer GetOrdererTrimToUpper(OrdererDto ordererCreate)
        {
            return GetOrderers().Where(x => x.Description.Trim().ToUpper() == ordererCreate.Description.TrimEnd().ToUpper())
               .FirstOrDefault();
        }

        public bool OrdererExist(int id)
        {
            return _context.Orderers.Any(p => p.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateOrderer(Orderer orderer)
        {
            _context.Update(orderer);
            return Save();
        }
    }
}
