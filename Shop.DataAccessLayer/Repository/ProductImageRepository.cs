using Shop.DataAccessLayer.Data;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;

namespace Shop.DataAccessLayer.Repository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private readonly ApplicationDbContext db;
        
        public ProductImageRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(ProductImage productImage)
        {
            dbSet.Update(productImage);
        }
    }
}
