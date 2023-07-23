using Shop.DataAccessLayer.Data;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;

namespace Shop.DataAccessLayer.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext db;
        
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            dbSet.Update(shoppingCart);
        }
    }
}
