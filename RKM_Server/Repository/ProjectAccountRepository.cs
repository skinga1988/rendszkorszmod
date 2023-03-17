using RKM_Server.Data;
using RKM_Server.DTO;
using RKM_Server.Interfaces;
using RKM_Server.Models;
using System.Text;
using System.Security.Cryptography;

namespace RKM_Server.Repository
{
    public class ProjectAccountRepository : ProjectAccountInterface
    {
        private readonly AppDbContext _context;
        public ProjectAccountRepository(AppDbContext context)
        {
            _context = context;
        }
        public bool CreateProjectAccount(ProjectAccount projectAccount)
        {
            _context.Add(projectAccount);
            return Save();
        }

        public bool DeleteProjectAccount(ProjectAccount projectAccount)
        {
            _context.Remove(projectAccount);
            return Save();
        }

        public ProjectAccount GetProjectAccount(int id)
        {
            return _context.ProjectAccounts.Where(po => po.Id == id).FirstOrDefault();
        }

        public ICollection<ProjectAccount> GetProjectAccounts()
        {
            return _context.ProjectAccounts.OrderBy(o => o.Id).ToList();
        }

        public ProjectAccount GetProjectAccountTrimToUpper(ProjectAccountDto projectAccountCreate)
        {
            throw new NotImplementedException();
        }

        public bool ProjectAccountExist(int id)
        {
            return _context.ProjectAccounts.Any(p => p.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateProjectAccount(ProjectAccount projectAccount)
        {
            _context.Update(projectAccount);
            return Save();
        }
    }
}
