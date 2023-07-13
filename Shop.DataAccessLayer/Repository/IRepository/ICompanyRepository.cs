using Shop.Models;

namespace Shop.DataAccessLayer.Repository.IRepository
{
    public interface ICompanyRepository :IRepository<Company>
    {
        void Update(Company company);
    }
}
