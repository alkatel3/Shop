using Shop.Models;

namespace Shop.DataAccessLayer.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
    }
}
