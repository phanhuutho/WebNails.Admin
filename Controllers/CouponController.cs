using Dapper;
using Newtonsoft.Json;
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
using WebNails.Admin.Utilities;

namespace WebNails.Admin.Controllers
{
    public class CouponController : Controller
    {
        private readonly string TokenKeyAPI = ConfigurationManager.AppSettings["TokenKeyAPI"];
        private INailCouponRepository _nailCouponRepository;
        private INailRepository _nailRepository;
        private IActionDetailRepository _actionDetailRepository;
        private INailAccountRepository _nailAccountRepository;
        private INailApiRepository _nailApiRepository;
        public CouponController(INailCouponRepository nailCouponRepository, INailRepository nailRepository, IActionDetailRepository actionDetailRepository, INailAccountRepository nailAccountRepository, INailApiRepository nailApiRepository)
        {
            _nailCouponRepository = nailCouponRepository;
            _nailRepository = nailRepository;
            _actionDetailRepository = actionDetailRepository;
            _nailAccountRepository = nailAccountRepository;
            _nailApiRepository = nailApiRepository;
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

                var objResult = _nailCouponRepository.GetNailCoupons(param);

                ViewBag.Count = param.Get<int>("@intTotalRecord");

                //get info nail
                var objNail = _nailRepository.GetNailByID(Nail_ID);
                Session.Add("Cur_Domain",objNail.Domain);
                Session.Add("Cur_NailName",objNail.Name);
                Session.Add("Cur_NailID",objNail.ID);

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
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailRepository.InitConnection(sqlConnect);
                _nailApiRepository.InitConnection(sqlConnect);

                var intNailID = int.Parse(Session["Cur_NailID"].ToString());
                var objNail = _nailRepository.GetNailByID(intNailID);
                if (objNail != null && objNail.NailApi_ID != null && objNail.NailApi_ID > 0)
                {
                    var objNailApi = _nailApiRepository.GetNailApiByID(objNail.NailApi_ID ?? 0);
                    if (!string.IsNullOrEmpty(objNailApi.Url) && objNailApi.Token != null)
                    {
                        var Token = new { Token = objNailApi.Token, Domain = objNail.Domain, TimeExpire = DateTime.Now.AddMinutes(5) };
                        var jsonStringToken = JsonConvert.SerializeObject(Token);
                        var strEncrypt = Sercurity.EncryptString(TokenKeyAPI, jsonStringToken);
                        return Redirect(string.Format("{0}/Coupon/Credit/{1}?token={2}&Cur_NailID={3}&Username={4}", objNailApi.Url, ID, strEncrypt, intNailID, User.Identity.Name));
                    }
                }

                if (ID == 0)
                {
                    return View(new NailCoupon() { ID = 0, Nail_ID = (int)Session["Cur_NailID"] });
                }
                else
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
                if (intCount > 0)
                {
                    var isCreate = item.ID == 0;
                    item.ID = isCreate ? intCount : item.ID;
                    _nailAccountRepository.InitConnection(sqlConnect);
                    var objAccount = _nailAccountRepository.GetNailAccountByUsername(User.Identity.Name);

                    _actionDetailRepository.InitConnection(sqlConnect);
                    _actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "NAIL_COUPON", UserID = objAccount.ID, Description = $"{(isCreate ? "Thêm" : "Sửa")} thông tin Coupon - " + Session["Cur_NailName"], DataJson = JsonConvert.SerializeObject(item), Field = "Nail_ID", FieldValue = item.Nail_ID });

                    return Json($"{(isCreate ? "Thêm" : "Sửa")} thông tin Coupon - " + Session["Cur_NailName"] + " thành công", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json($"{(item.ID == 0 ? "Thêm" : "Sửa")} thông tin Coupon - " + Session["Cur_NailName"] + " thất bại", JsonRequestBehavior.AllowGet);
                }
            }
        }

        [Token]
        public ActionResult ApiCredit(int ID)
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailCouponRepository.InitConnection(sqlConnect);
                var objNailCoupon = _nailCouponRepository.GetNailCouponByID(ID);
                return Json(objNailCoupon, JsonRequestBehavior.AllowGet);
            }
        }

        [Token]
        [HttpPost]
        public ActionResult ApiCredit(NailCoupon item, string Cur_NailName, string Username)
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailCouponRepository.InitConnection(sqlConnect);
                var intCount = _nailCouponRepository.SaveChange(item);
                if (intCount > 0)
                {
                    var isCreate = item.ID == 0;
                    item.ID = isCreate ? intCount : item.ID;
                    _nailAccountRepository.InitConnection(sqlConnect);
                    var objAccount = _nailAccountRepository.GetNailAccountByUsername(Username);

                    _actionDetailRepository.InitConnection(sqlConnect);
                    _actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "NAIL_COUPON", UserID = objAccount.ID, Description = $"{(isCreate ? "Thêm" : "Sửa")} thông tin Coupon - " + Cur_NailName, DataJson = JsonConvert.SerializeObject(item), Field = "Nail_ID", FieldValue = item.Nail_ID });

                    return Json(1);
                }
                else
                {
                    return Json(0);
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
                    var item = _nailCouponRepository.GetNailCouponByID(ID);
                    var intCount = _nailCouponRepository.DeleteNailCoupon(ID);
                    if (intCount == 1)
                    {
                        _nailAccountRepository.InitConnection(sqlConnect);
                        var objAccount = _nailAccountRepository.GetNailAccountByUsername(User.Identity.Name);

                        _actionDetailRepository.InitConnection(sqlConnect);
                        _actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "NAIL_COUPON", UserID = objAccount.ID, Description = "Xóa Coupon - " + Session["Cur_NailName"], DataJson = JsonConvert.SerializeObject(item), Field = "Nail_ID", FieldValue = item.Nail_ID });

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

                var objResult = _nailCouponRepository.GetNailCoupons(param);

                ViewBag.Count = param.Get<int>("@intTotalRecord");

                return PartialView(objResult);
            }
        }
    }
}