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
    public class PricesController : Controller
    {
        private INailPricesRepository _nailPricesRepository;
        private INailRepository _nailRepository;
        public PricesController(INailPricesRepository nailPricesRepository, INailRepository nailRepository)
        {
            _nailPricesRepository = nailPricesRepository;
            _nailRepository = nailRepository;
        }
        // GET: Prices
        [Authorize]
        public ActionResult Index(int Nail_ID = 0)
        {
            if (Nail_ID == 0)
            {
                return RedirectToAction("Index", "ControlPanel");
            }
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                var intSkip = Utilities.PagingHelper.Skip;

                var param = new DynamicParameters();
                param.Add("@intSkip", intSkip);
                param.Add("@intTake", Utilities.PagingHelper.CountSort);
                param.Add("@intNail_ID", Nail_ID);
                param.Add("@intTotalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);

                _nailPricesRepository.InitConnection(sqlConnect);
                _nailRepository.InitConnection(sqlConnect);

                var objResult = _nailPricesRepository.GetNailPricesByNailID(param);

                ViewBag.Count = param.Get<int>("@intTotalRecord");

                //get info nail
                var objNail = _nailRepository.GetNailByID(Nail_ID);
                Session["Cur_Domain"] = objNail.Domain;
                Session["Cur_NailName"] = objNail.Name;
                Session["Cur_NailID"] = objNail.ID;

                return View(objResult);
            }
        }

        [Authorize]
        public ActionResult Credit(int ID = 0)
        {
            if (Session["Cur_NailID"] == null || Session["Cur_Domain"] == null || Session["Cur_NailName"] == null)
            {
                return RedirectToAction("Index", "Nail");
            }
            if (ID == 0)
            {
                return View(new NailPrices() { ID = 0, Nail_ID = (int)Session["Cur_NailID"] });
            }
            else
            {
                using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
                {
                    _nailPricesRepository.InitConnection(sqlConnect);
                    var objNailCoupon = _nailPricesRepository.GetNailPricesByID(ID);
                    return View(objNailCoupon);
                }
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Credit(NailPrices item)
        {
            if (Session["Cur_NailID"] == null || Session["Cur_Domain"] == null || Session["Cur_NailName"] == null)
            {
                return RedirectToAction("Index", "Nail");
            }
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailPricesRepository.InitConnection(sqlConnect);
                var intCount = _nailPricesRepository.SaveChange(item);
                if (intCount == 1)
                {
                    return Json($"{(item.ID == 0 ? "Thêm" : "Sửa")} thông tin Prices List - " + Session["Cur_NailName"] + " thành công", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json($"{(item.ID == 0 ? "Thêm" : "Sửa")} thông tin Prices List - " + Session["Cur_NailName"] + " thất bại", JsonRequestBehavior.AllowGet);
                }
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(int ID = 0)
        {
            if (Session["Cur_NailID"] == null || Session["Cur_Domain"] == null || Session["Cur_NailName"] == null)
            {
                return RedirectToAction("Index", "Nail");
            }
            if (ID == 0)
            {
                return RedirectToAction("Index", "ControlPanel");
            }
            else
            {
                using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
                {
                    _nailPricesRepository.InitConnection(sqlConnect);
                    var intCount = _nailPricesRepository.DeleteNailPrices(ID);
                    if (intCount == 1)
                    {
                        return Json("Xóa thành công Prices List", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Xóa thất bại Prices List", JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        [Authorize]
        public ActionResult GetTable(int Nail_ID)
        {
            if (Nail_ID == 0)
            {
                return RedirectToAction("Index", "Nail");
            }
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                var intSkip = Utilities.PagingHelper.Skip;

                var param = new DynamicParameters();
                param.Add("@intSkip", intSkip);
                param.Add("@intTake", Utilities.PagingHelper.CountSort);
                param.Add("@intNail_ID", Nail_ID);
                param.Add("@intTotalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);

                _nailPricesRepository.InitConnection(sqlConnect);
                _nailRepository.InitConnection(sqlConnect);

                var objResult = _nailPricesRepository.GetNailPricesByNailID(param);

                ViewBag.Count = param.Get<int>("@intTotalRecord");

                return PartialView(objResult);
            }
        }
    }
}