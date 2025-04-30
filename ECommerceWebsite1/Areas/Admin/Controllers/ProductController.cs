using ECommerceWebsite.DataAccessLayer.Infrastructure.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using ECommerceWebsite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ECommerceWebsite.Models;

namespace ECommerceWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment, ILogger<ProductController> logger)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }
        #region APICALL
        public IActionResult AllProducts()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties:"Category");
            return Json(new { data = products });
        }
        #endregion
        public IActionResult Index()
        {
            //ProductVM productVM = new ProductVM();
            //productVM.Products = _unitOfWork.Product.GetAll();
            return View();
        }

        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            ProductVM vm = new ProductVM()
            {
                Product= new(),
                    Categories=_unitOfWork.Category.GetAll().Select(x=>new SelectListItem()
                    {
                        Text= x.Name,
                        Value=x.Id.ToString()
                    })
            };
            if (!id.HasValue || id == 0)
            {
                return View(vm);
            }
            else
            {
                vm.Product = _unitOfWork.Product.GetT(x => x.Id == id);
                if (vm.Product == null)
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
        public IActionResult CreateUpdate(ProductVM vm, IFormFile? file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string fileName = string.Empty;
                    if (file != null)
                    {
                        string uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "ProductImage");
                        Directory.CreateDirectory(uploadDir); // Ensure folder exists

                        fileName = Guid.NewGuid().ToString() + "-" + Path.GetFileName(file.FileName); // Secure filename
                        string filePath = Path.Combine(uploadDir, fileName);

                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(vm.Product.ImageUrl))
                        {
                            _logger.LogInformation("Processing product image: {FileName}", fileName);
                            var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, vm.Product.ImageUrl.TrimStart(Path.DirectorySeparatorChar));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        vm.Product.ImageUrl = Path.Combine("ProductImage", fileName).Replace("\\", "/"); // URL-safe
                    }

                    if (vm.Product.Id == 0)
                    {
                        _logger.LogInformation("Creating new product: {ProductName}", vm.Product.Name);
                        _unitOfWork.Product.Add(vm.Product);
                        TempData["Success"] = "Product created successfully.";
                    }
                    else
                    {
                        _logger.LogInformation("Updating product: {ProductId}", vm.Product.Id);
                        _unitOfWork.Product.Update(vm.Product);
                        TempData["Success"] = "Product updated successfully.";
                    }

                    _unitOfWork.Save();
                    _logger.LogDebug("Product changes saved successfully.");
                    return RedirectToAction("Index");
                }

                _logger.LogWarning("Invalid model state in Product CreateUpdate.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Product CreateUpdate.");
                
                return RedirectToAction("Index");
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

        //[HttpGet]
        //public IActionResult Delete(int? id)
        //{
        //    if (!id.HasValue || id == 0)
        //    {
        //        return NotFound();
        //    }

        //    var category = _unitOfWork.Category.GetT(x => x.Id == id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(category);
        //}

        #region DeleteAPICALL
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = _unitOfWork.Product.GetT(x => x.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error in fetching data" });
            }
            else
            {
                var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                _unitOfWork.Product.Delete(product);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Product Deleted" });

            }



        }
        #endregion
    }
}
