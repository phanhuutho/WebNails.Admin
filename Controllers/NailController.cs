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
using WebNails.Admin.Utilities;

namespace WebNails.Admin.Controllers
{
    public class NailController : Controller
    {
        private INailRepository _nailRepository;
        private INailCouponRepository _nailCouponRepository;
        private INailPricesRepository _nailPricesRepository;
        public NailController(INailRepository nailRepository, INailCouponRepository nailCouponRepository, INailPricesRepository nailPricesRepository)
        {
            _nailRepository = nailRepository;
            _nailCouponRepository = nailCouponRepository;
            _nailPricesRepository = nailPricesRepository;
        }

        // GET: Nail
        [Authorize]
        public ActionResult Index(string Search = "")
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                var intSkip = Utilities.PagingHelper.Skip;

                var param = new DynamicParameters();
                param.Add("@intSkip", intSkip);
                param.Add("@intTake", Utilities.PagingHelper.CountSort);
                param.Add("@strValue", Search);
                param.Add("@intTotalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);

                _nailRepository.InitConnection(sqlConnect);

                var objResult = _nailRepository.GetNails(param);

                ViewBag.Count = param.Get<int>("@intTotalRecord");

                Session.Remove("Cur_Domain");
                Session.Remove("Cur_NailName");
                Session.Remove("Cur_NailID");

                return View(objResult);
            }    
        }

        [Authorize]
        public ActionResult Credit(int ID = 0)
        {
            if(ID == 0)
            {
                Session["Cur_Domain"] = "Temp";
                return View(new Nail() { ID = 0, Name = "" });
            }
            else
            {
                using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
                {
                    _nailRepository.InitConnection(sqlConnect);
                    var objNail = _nailRepository.GetNailByID(ID);
                    Session["Cur_Domain"] = objNail.Domain;
                    return View(objNail);
                }
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Credit(Nail item)
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailRepository.InitConnection(sqlConnect);
                var intCount = _nailRepository.SaveChange(item);
                if (intCount == 1)
                {
                    return Json($"{(item.ID == 0 ? "Thêm" : "Sửa")} thông tin " + item.Name + " thành công", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json($"{(item.ID == 0 ? "Thêm" : "Sửa")} thông tin " + item.Name + " thất bại", JsonRequestBehavior.AllowGet);
                }
            }    
        }

        [Authorize]
        public ActionResult SyncDataWeb(int ID)
        {
            if (ID == 0)
            {
                return RedirectToAction("Index", "ControlPanel");
            }
            else
            {
                using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
                {
                    _nailRepository.InitConnection(sqlConnect);
                    var objNail = _nailRepository.GetNailByID(ID);

                    _nailCouponRepository.InitConnection(sqlConnect);
                    var objNailCoupon = _nailCouponRepository.GetNailCouponsByNailID(ID).Select(x => new JsonCoupon { Src = x.URL, Status = x.Status }).ToList();

                    _nailPricesRepository.InitConnection(sqlConnect);
                    var objNailPrices = _nailPricesRepository.GetNailPricesByNailID(ID).Select(x => new JsonPrice { Src = x.URL, Status = x.Status }).ToList();


                    var jsonInfo = new JsonInfo
                    {
                        Name = objNail.Name,
                        Logo = objNail.Logo,
                        HyperLinkTell = objNail.HyperLinkTell,
                        TextTell = objNail.TextTell,
                        LinkBookingAppointment = objNail.LinkBookingAppointment,
                        GooglePlus = objNail.GooglePlus,
                        Address = objNail.Address,
                        LinkGoogleMapAddress = objNail.LinkGoogleMapAddress,
                        LinkIFrameGoogleMap = objNail.LinkIFrameGoogleMap,
                        ShowCoupon = objNail.Coupons,
                        Coupons = objNailCoupon,
                        Prices = objNailPrices,
                        Telegram = new JsonSocial(),
                        Facebook = new JsonSocial(),
                        Instagram = new JsonSocial(),
                        Twitter = new JsonSocial(),
                        Youtube = new JsonSocial()

                    };
                    Commons.GenerateDataWeb(jsonInfo, objNail.BusinessHours, objNail.AboutUs, objNail.AboutUsHome);

                    return Json("Cập nhật dữ liệu lên website thành công !", JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}