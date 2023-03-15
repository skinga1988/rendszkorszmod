using RKM_Server.DTO;
using RKM_Server.Models;

namespace RKM_Server.Interfaces
{
    public interface UserInterface
    {
        ICollection<User> GetUsers();
        User GetOwnerOfAProject(int project_id);
        User GetModifierOfAProject(int projectaccount_id);
        User GetModifierOfAStockAccount(int stockaccount_id);
        User GetUser(string username);
        User GetUser(int id);
        bool UserExist(string username);
        bool UserExist(int id);
        bool CreateUser(User user);
        bool UpdateUser(User user);
        User GetUserTrimToUpper(UserDto userCreate);
        bool DeleteUser(User user);
        bool Save();
        bool CreateUser(UserDto userMap);
    }
}
