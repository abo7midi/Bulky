using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles=SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public IWebHostEnvironment _webHostEnvironment { get; }

        public ProductController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: ProductController
        public ActionResult Index()
        {

            List<Product> ProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            //EF Projections technique :use select to convert Category Class to (SelectListItem) Datatype 
            return View(ProductList);
        }

        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductController/Create
        public ActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            ProductVM productVM = new()
            {
                Product=new Product(),
                CategoryList=CategoryList
            };
            if (id==null || id==0)
            {
                //Create
                return View(productVM);
            }
            else
            {
                //Update
                productVM.Product = _unitOfWork.Product.Get(prod => prod.Id == id,includeProperties:"ProductImages");
                return View(productVM);
            }
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(ProductVM productVM,List<IFormFile> files)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (productVM.Product.Id == 0)
                    {
                        _unitOfWork.Product.Add(productVM.Product);
                    }
                    else
                    {
                        _unitOfWork.Product.Update(productVM.Product);
                    }
                    _unitOfWork.Save();


                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    if (files != null)
                    {
                        foreach(IFormFile file in files)
                        {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            string productPath = @"images\products\product-" + productVM.Product.Id;
                            string finalPath = Path.Combine(wwwRootPath,productPath);
                         
                            if(!Directory.Exists(finalPath))
                                Directory.CreateDirectory(finalPath);

                            using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }

                            ProductImage productImage = new()
                            {
                                ImageUrl = @"\" + productPath + @"\" + fileName,
                                ProductId = productVM.Product.Id,
                            };

                            //في حال ان المنتج لا توجد له صور من قبل يعني في حاله اضافة منتج جديد
                            if(productVM.Product.ProductImages==null)
                                productVM.Product.ProductImages=new List<ProductImage>();   //عشان لا يرجع لنا رسالة اخطأ اننا لم نضف صور

                            productVM.Product.ProductImages.Add(productImage);

                        }
                        _unitOfWork.Product.Update(productVM.Product);
                        _unitOfWork.Save();
                    }
                    TempData["success"] = "Product Created/Updated Successfully";
                    return RedirectToAction("Index");
                }else
                {
                    IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                    productVM.CategoryList = CategoryList;
                    return View(productVM);
                }
            }
            catch
            {
                return View();
            }
        }

        public IActionResult DeleteImage(int ImageId)
        {
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == ImageId);
            var productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath=Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\'));
                    if(System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Image Deleted Successfully";
            }
            return RedirectToAction(nameof(Upsert),new {id=productId});
        }
        
        // GET: ProductController/Delete/5
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? productFromDb = _unitOfWork.Product.Get(prod => prod.Id == id);
        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDb);
        //}
        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    Product? product = _unitOfWork.Product.Get(prod => prod.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Product.Remove(product);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product Deleted Successfully";

        //    return RedirectToAction("Index");

        //}
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> ProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = ProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(p => p.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While Deleting"});
            }
            //var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
            //if (System.IO.File.Exists(oldImagePath))
            // {
            //     System.IO.File.Delete(oldImagePath);
            // }
            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if(Directory.Exists(finalPath))
            {
                Directory.Delete(finalPath, true);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new {success=true,message="Delete Successful"});
        }
        #endregion
    }
}
