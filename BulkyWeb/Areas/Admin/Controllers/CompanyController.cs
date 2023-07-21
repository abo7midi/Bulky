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
    //[Authorize(Roles=SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: CompanyController
        public ActionResult Index()
        {

            List<Company> CompanyList = _unitOfWork.Company.GetAll().ToList();
            //EF Projections technique :use select to convert Category Class to (SelectListItem) Datatype 
            return View(CompanyList);
        }

        // GET: CompanyController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CompanyController/Create
        public ActionResult Upsert(int? id)
        {
            if (id==null || id==0)
            {
                //Create
                return View(new Company());
            }
            else
            {
                //Update
                Company company=_unitOfWork.Company.Get(u=>u.Id==id);
                return View(company);
            }
        }

        // POST: CompanyController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(Company company)
        {
            try
            {
                if (ModelState.IsValid)
                { 
                    if(company.Id==0)
                    {
                        _unitOfWork.Company.Add(company);
                    }
                    else
                    {
                        _unitOfWork.Company.Update(company);
                    }
                    _unitOfWork.Save();
                    TempData["success"] = "Company Created Successfully";
                    return RedirectToAction("Index");
                }else
                {
                    return View(company);
                }
            }
            catch
            {
                return View();
            }
        }

        
        // GET: CompanyController/Delete/5
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Company? CompanyFromDb = _unitOfWork.Company.Get(prod => prod.Id == id);
        //    if (CompanyFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(CompanyFromDb);
        //}
        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    Company? Company = _unitOfWork.Company.Get(prod => prod.Id == id);
        //    if (Company == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Company.Remove(Company);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Company Deleted Successfully";

        //    return RedirectToAction("Index");

        //}
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> CompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = CompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(p => p.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While Deleting"});
            }
            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();

            return Json(new {success=true,message="Delete Successful"});
        }
        #endregion
    }
}
