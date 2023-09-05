﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.CommonHelper;
using MyApp.DataAccessLayer;
using MyApp.DataAccessLayer.Infrastructure.IRepository;
using MyApp.Models;
using MyApp.Models.ViewModels;

namespace MyAppWeb.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =WebSiteRole.Role_Admin)]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitofWork;

        public CategoryController(IUnitOfWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        public IActionResult Index()
        {
            CategoryVM categoryVM = new CategoryVM();
            categoryVM.categories = _unitofWork.Category.GetAll();
            return View(categoryVM);
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
        //    if (ModelState.IsValid) { 
        //    _unitofWork.Category.Add(category);
        //    _unitofWork.Save();
        //        TempData["success"] = "Category Created Done!";
        //    return RedirectToAction("Index");
        //    }
        //    return View(category);
        //}


        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            CategoryVM vm = new CategoryVM();
            if (id==null || id==0)
            {
                return View(vm);
            }
            else
            {
                vm.Category = _unitofWork.Category.GetT(x => x.Id == id);
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
            if (ModelState.IsValid)
            {
                if (vm.Category.Id==0)
                {
                    _unitofWork.Category.Add(vm.Category);
                    TempData["success"] = "Category Created Done!";
                }
                else
                {
                    _unitofWork.Category.Update(vm.Category);
                    TempData["success"] = "Category Updated Done!";

                }
               
                _unitofWork.Save();
               
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _unitofWork.Category.GetT(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteData(int? id)
        {
            var category = _unitofWork.Category.GetT(x => x.Id == id);
            if (category==null)
            {
                return NotFound();
            }
           _unitofWork.Category.Delete(category);
            _unitofWork.Save();
            TempData["success"] = "Category Deleted Done!";
            return RedirectToAction("Index");
        }







    }
}
