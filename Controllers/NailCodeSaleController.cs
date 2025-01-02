using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebNails.Admin.Interfaces;
using WebNails.Admin.Models;

namespace WebNails.Admin.Controllers
{
    public class NailCodeSaleController : Controller
    {
        private INailCodeSaleRepository _nailNailCodeSaleRepository;
        private INailRepository _nailRepository;
        public NailCodeSaleController(INailCodeSaleRepository nailNailCodeSaleRepository, INailRepository nailRepository)
        {
            this._nailNailCodeSaleRepository = nailNailCodeSaleRepository;
            this._nailRepository = nailRepository;
        }
        // GET: NailCodeSale
        [Authorize]
        public ActionResult Index(int Nail_ID = 0, string Search = "")
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                var intSkip = Utilities.PagingHelper.Skip;

                var param = new DynamicParameters();
                param.Add("@intSkip", intSkip);
                param.Add("@intTake", Utilities.PagingHelper.CountSort);
                param.Add("@strValue", Search);
                param.Add("@intNail_ID", Nail_ID);
                param.Add("@intTotalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);

                _nailNailCodeSaleRepository.InitConnection(sqlConnect);

                var objResult = _nailNailCodeSaleRepository.GetNailCodeSales(param);

                ViewBag.Count = param.Get<int>("@intTotalRecord");
                ViewBag.NailID = Nail_ID;

                return View(objResult);
            }
        }
        [Authorize]
        public ActionResult Credit(int ID = 0, int Nail_ID = 0)
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailRepository.InitConnection(sqlConnect);
                if (ID == 0)
                {
                    return View(new NailCodeSale() { ID = 0, Nail_ID = Nail_ID, ExpireDateFrom = DateTime.Now.AddDays(1), ExpireDateTo = DateTime.Now.AddDays(2), Nails = _nailRepository.GetNails().Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Domain }).OrderBy(x => x.Text).ToList() });
                }
                else
                {
                    _nailNailCodeSaleRepository.InitConnection(sqlConnect);
                    var objNailCodeSale = _nailNailCodeSaleRepository.GetNailCodeSaleByID(ID);
                    objNailCodeSale.Nails = _nailRepository.GetNails().Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Domain, Selected = x.ID == objNailCodeSale.Nail_ID }).OrderBy(x => x.Text).ToList();
                    return View(objNailCodeSale);
                }
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Credit(NailCodeSale item)
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailNailCodeSaleRepository.InitConnection(sqlConnect);
                var intCount = _nailNailCodeSaleRepository.SaveChange(item);
                if (intCount > 0)
                {
                    var isCreate = item.ID == 0;
                    return Json($"{(isCreate ? "Thêm" : "Sửa")} thông tin Code Sale thành công", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json($"{(item.ID == 0 ? "Thêm" : "Sửa")} thông tin Code Sale thất bại", JsonRequestBehavior.AllowGet);
                }
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(int ID = 0)
        {
            if (ID == 0)
            {
                return RedirectToAction("Index", "ControlPanel");
            }
            else
            {
                using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
                {
                    _nailNailCodeSaleRepository.InitConnection(sqlConnect);
                    var intCount = _nailNailCodeSaleRepository.DeleteNailCodeSale(ID);
                    if (intCount == 1)
                    {
                        return Json("Xóa thành công Code Sale", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Xóa thất bại Code Sale", JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        [Authorize]
        public ActionResult GetTable(int Nail_ID = 0, string Search = "")
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                var intSkip = Utilities.PagingHelper.Skip;

                var param = new DynamicParameters();
                param.Add("@intSkip", intSkip);
                param.Add("@intTake", Utilities.PagingHelper.CountSort);
                param.Add("@strValue", Search);
                param.Add("@intNail_ID", Nail_ID);
                param.Add("@intTotalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);

                _nailNailCodeSaleRepository.InitConnection(sqlConnect);

                var objResult = _nailNailCodeSaleRepository.GetNailCodeSales(param);

                ViewBag.Count = param.Get<int>("@intTotalRecord");

                return PartialView(objResult);
            }
        }
    }
}