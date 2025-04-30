using ECommerceWebsite.DataAccessLayer.Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebsite.Areas.Customer.Controllers
{
    [Area( "Customer")]
    public class CartController : Controller
    {
        private UnitOfWork _unitOfWork;

        public CartController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
