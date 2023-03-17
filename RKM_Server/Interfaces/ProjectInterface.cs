using RKM_Server.DTO;
using RKM_Server.Models;

namespace RKM_Server.Interfaces
{
    public interface ProjectInterface
    {
        ICollection<Project> GetProjects();
        Project GetProject(int id);
        bool ProjectExist(int id);
        bool CreateProject(Project project);
        bool UpdateProject(Project project);
        Project GetProjectTrimToUpper(ProjectDto projectCreate);
        bool DeleteProject(Project project);
        bool Save();
    }
}
