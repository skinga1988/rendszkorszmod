using RKM_Server.Data;
using RKM_Server.DTO;
using RKM_Server.Interfaces;
using RKM_Server.Models;
using System.Text;
using System.Security.Cryptography;

namespace RKM_Server.Repository
{
    public class ProjectRepository : ProjectInterface
    {
        private readonly AppDbContext _context;
        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }
        public bool CreateProject(Project project)
        {
            _context.Add(project);
            return Save();
        }

        public bool DeleteProject(Project project)
        {
            _context.Remove(project);
            return Save();
        }

        public Project GetProject(int id)
        {
            return _context.Projects.Where(p => p.Id == id).FirstOrDefault();
        }

        public ICollection<Project> GetProjects()
        {
            return _context.Projects.OrderBy(p => p.Id).ToList();
        }

        public Project GetProjectTrimToUpper(ProjectDto projectCreate)
        {
            return GetProjects().Where(x => x.ProjectDescription.Trim().ToUpper() == projectCreate.ProjectDescription.TrimEnd().ToUpper())
               .FirstOrDefault();
        }

        public bool ProjectExist(int id)
        {
            return _context.Projects.Any(p => p.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateProject(Project project)
        {
            _context.Update(project);
            return Save();
        }
    }
}
