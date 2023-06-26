using Shop.Models;

namespace Shop.DataAccessLayer.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
    }
}
