using Shop.DataAccessLayer.Data;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;

namespace Shop.DataAccessLayer.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(Product product)
        {
            var productDb = db.Products.FirstOrDefault(u => u.Id == product.Id);
            if(productDb != null)
            {
                productDb.Title = product.Title;
                productDb.ISBN = product.ISBN;
                productDb.Price = product.Price;
                productDb.Price50 = product.Price50;
                productDb.Price100 = product.Price100;
                productDb.Description = product.Description;
                productDb.CategoryId = product.CategoryId;
                productDb.Author = product.Author;
                if(product.ImageUrl != null)
                {
                    productDb.ImageUrl = product.ImageUrl;
                }
            }
        }
    }
}
