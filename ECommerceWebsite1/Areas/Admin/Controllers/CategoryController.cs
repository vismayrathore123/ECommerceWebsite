using ECommerceWebsite.DataAccessLayer.Infrastructure.IRepository;
using ECommerceWebsite.Models;
using ECommerceWebsite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(IUnitOfWork unitOfWork, ILogger<CategoryController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            CategoryVM categoryVM = new CategoryVM();
          categoryVM.categories = _unitOfWork.Category.GetAll();
            return View(categoryVM);
        }

        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            CategoryVM vm= new CategoryVM();
            if (!id.HasValue || id == 0)
            {
                return View (vm);
            }
            else
            {
                vm.Category = _unitOfWork.Category.GetT(x => x.Id == id);
                if (vm.Category == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(vm);
                }
   
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUpdate(CategoryVM vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.Category.Id == 0)
                    {
                        _logger.LogInformation("Creating new category: {CategoryName}", vm.Category.Name);
                        _unitOfWork.Category.Add(vm.Category);
                        TempData["Success"] = "Category Created Done!";
                    }
                    else
                    {
                        _logger.LogInformation("Updating category ID {CategoryId}", vm.Category.Id);
                        _unitOfWork.Category.Update(vm.Category);
                        TempData["Success"] = "Category Updated Done!";
                    }

                    _unitOfWork.Save();
                    _logger.LogDebug("Category changes saved successfully");
                    return RedirectToAction("Index");
                }

                _logger.LogWarning("Invalid model state in Category CreateUpdate");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Category CreateUpdate");
                throw;
            }
        }

        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Category.Add(category);
        //        _unitOfWork.Save();
        //        TempData["Success"] = "Category Created Done!";
        //        return RedirectToAction("Index");
        //    }
        //    return View(category);
        //}

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue || id == 0)
            {
                return NotFound();
            }

            var category = _unitOfWork.Category.GetT(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteData(int? id)
        {
            var category = _unitOfWork.Category.GetT(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Delete(category);
            _unitOfWork.Save();
            TempData["Success"] = "Category Deleted Done!";
            return RedirectToAction("Index");
        }
    }
}
