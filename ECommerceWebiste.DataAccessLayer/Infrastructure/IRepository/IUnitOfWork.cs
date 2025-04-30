using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceWebsite.DataAccessLayer.Infrastructure.IRepository;

namespace ECommerceWebsite.DataAccessLayer.Infrastructure.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product  { get; }
        IApplicationUser ApplicationUser { get; }
        ICartRepository Cart { get; } 
        void Save();
    }
}
