using Shop.DataAccessLayer.Data;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;

namespace Shop.DataAccessLayer.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(Company company)
        {
            dbSet.Update(company);
        }
    }
}
