using RKM_Server.DTO;
using RKM_Server.Models;

namespace RKM_Server.Interfaces
{
    public interface ProjectAccountInterface
    {
        ICollection<ProjectAccount> GetProjectAccounts();
        ProjectAccount GetProjectAccount(int id);
        bool ProjectAccountExist(int id);
        bool CreateProjectAccount(ProjectAccount projectAccount);
        bool UpdateProjectAccount(ProjectAccount projectAccount);
        ProjectAccount GetProjectAccountTrimToUpper(ProjectAccountDto projectAccountCreate);
        bool DeleteProjectAccount(ProjectAccount projectAccount);
        bool Save();
    }
}
