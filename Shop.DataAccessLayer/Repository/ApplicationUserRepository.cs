using Shop.DataAccessLayer.Data;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;

namespace Shop.DataAccessLayer.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(ApplicationUser applicationUser)
        {
            dbSet.Update(applicationUser);
        }
    }
}
