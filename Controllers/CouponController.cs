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
    public class CouponController : Controller
    {
        private INailCouponRepository _nailCouponRepository;
        private INailRepository _nailRepository;
        public CouponController(INailCouponRepository nailCouponRepository, INailRepository nailRepository)
        {
            _nailCouponRepository = nailCouponRepository;
            _nailRepository = nailRepository;
        }
        // GET: Coupon
        [Authorize]
        public ActionResult Index(int Nail_ID = 0)
        {
            if(Nail_ID == 0)
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

                _nailCouponRepository.InitConnection(sqlConnect);
                _nailRepository.InitConnection(sqlConnect);

                var objResult = _nailCouponRepository.GetNailCouponsByNailID(param);

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
            if(Session["Cur_NailID"] == null || Session["Cur_Domain"] == null || Session["Cur_NailName"] == null)
            {
                return RedirectToAction("Index", "Nail");
            }    
            if (ID == 0)
            {
                return View(new NailCoupon() { ID = 0, Nail_ID = (int)Session["Cur_NailID"] });
            }
            else
            {
                using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
                {
                    _nailCouponRepository.InitConnection(sqlConnect);
                    var objNailCoupon = _nailCouponRepository.GetNailCouponByID(ID);
                    return View(objNailCoupon);
                }
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Credit(NailCoupon item)
        {
            if (Session["Cur_NailID"] == null || Session["Cur_Domain"] == null || Session["Cur_NailName"] == null)
            {
                return RedirectToAction("Index", "Nail");
            }
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailCouponRepository.InitConnection(sqlConnect);
                var intCount = _nailCouponRepository.SaveChange(item);
                if (intCount == 1)
                {
                    return Json($"{(item.ID == 0 ? "Thêm" : "Sửa")} thông tin Coupon cho " + Session["Cur_NailName"] + " thành công", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json($"{(item.ID == 0 ? "Thêm" : "Sửa")} thông tin Coupon cho " + Session["Cur_NailName"] + " thất bại", JsonRequestBehavior.AllowGet);
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
                    _nailCouponRepository.InitConnection(sqlConnect);
                    var intCount = _nailCouponRepository.DeleteNailCoupon(ID);
                    if (intCount == 1)
                    {
                        return Json("Xóa thành công Coupon", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Xóa thất bại Coupon", JsonRequestBehavior.AllowGet);
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

                _nailCouponRepository.InitConnection(sqlConnect);
                _nailRepository.InitConnection(sqlConnect);

                var objResult = _nailCouponRepository.GetNailCouponsByNailID(param);

                ViewBag.Count = param.Get<int>("@intTotalRecord");

                return PartialView(objResult);
            }
        }
    }
}