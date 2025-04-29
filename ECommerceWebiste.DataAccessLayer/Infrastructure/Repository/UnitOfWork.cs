
using ECommerceWebsite.DataAccessLayer.Infrastructure.IRepository;


namespace ECommerceWebsite.DataAccessLayer.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;
        public ICategoryRepository Category { get; private set; }

        public IProdcutRepository Product { get; private set; }
        public ICartRepository Cart { get; private set; }
        public IApplicationUser ApplicationUser { get; private set; }   
        public UnitOfWork(ApplicationDbContext context, ICartRepository cart)
        {
            _context = context;
            Category = new CategoryRepository(context);
            Product = new ProductRepository(context);
            Cart = new CartRepository(context);
            ApplicationUser = new ApplicationRepository(context);
        }

        public void Save()
        {
          _context.SaveChanges();
        }
    }
}
