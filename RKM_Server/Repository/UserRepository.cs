using RKM_Server.Data;
using RKM_Server.DTO;
using RKM_Server.Interfaces;
using RKM_Server.Models;
using System.Text;
using System.Security.Cryptography;

namespace RKM_Server.Repository
{
    public class UserRepository : UserInterface
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool CreateUser(User user)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(user.Password);
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);
            string passwordHash = Convert.ToBase64String(hashBytes);
            user.Password = passwordHash;
            _context.Add(user);
            return Save();
        }

        public bool CreateUser(UserDto userMap)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(User user)
        {
            _context.Remove(user);
            return Save();
        }

        public User GetModifierOfAProject(int projectaccount_id)
        {
            throw new NotImplementedException();
        }

        public User GetModifierOfAStockAccount(int stockaccount_id)
        {
            throw new NotImplementedException();
        }

        public User GetOwnerOfAProject(int project_id)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string username)
        {
            return _context.Users.Where(u => u.UserName == username).FirstOrDefault();
        }

        public User GetUser(int id)
        {
            return _context.Users.Where(u => u.UserId == id).FirstOrDefault();
        }


        public ICollection<User> GetUsers()
        {
            return _context.Users.OrderBy(u => u.UserId).ToList();
        }

        public User GetUserTrimToUpper(UserDto userCreate)
        {
            return GetUsers().Where(c => c.UserName.Trim().ToUpper() == userCreate.UserName.TrimEnd().ToUpper())
                .FirstOrDefault();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateUser(User user)
        {
            _context.Update(user);
            return Save();
        }

        public bool UserExist(string username)
        {
            return _context.Users.Any(u => u.UserName == username);
        }

        public bool UserExist(int id)
        {
            return _context.Users.Any(u => u.UserId == id);
        }
    }
}
