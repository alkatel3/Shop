using Shop.DataAccessLayer.Data;
using Shop.DataAccessLayer.Repository.IRepository;
using Shop.Models;

namespace Shop.DataAccessLayer.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext db;
        
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(OrderDetail orderDetail)
        {
            dbSet.Update(orderDetail);
        }
    }
}
