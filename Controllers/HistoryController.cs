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

namespace WebNails.Admin.Controllers
{
    public class HistoryController : Controller
    {
        private IActionDetailRepository _actionDetailRepository;
        public HistoryController(IActionDetailRepository actionDetailRepository)
        {
            _actionDetailRepository = actionDetailRepository;
        }
        // GET: History
        [Authorize]
        public ActionResult Index(int Nail_ID, string Tab = "NAIL")
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
                param.Add("@strTable", Tab);
                param.Add("@strField", (Tab == "NAIL" || Tab == "SyncDataWeb" ? "ID" : "Nail_ID"));
                param.Add("@intFieldValue", Nail_ID);
                param.Add("@intTotalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);

                _actionDetailRepository.InitConnection(sqlConnect);

                var objResult = _actionDetailRepository.GetActionDetails(param);

                ViewBag.Table = Tab;
                ViewBag.Nail_ID = Nail_ID;
                ViewBag.Count = param.Get<int>("@intTotalRecord");

                return View(objResult);
            }
        }

        [Authorize]
        public ActionResult GetDataDetail(Guid ID)
        {
            if (ID == null)
            {
                return RedirectToAction("Index", "Nail");
            }
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _actionDetailRepository.InitConnection(sqlConnect);
                var objDetail = _actionDetailRepository.GetActionDetail(ID);

                switch(objDetail.Table)
                {
                    case "NAIL":
                        var objNail = JsonConvert.DeserializeObject<Nail>(objDetail.DataJson);
                        return PartialView("DataDetail_Info", new Nail
                        {
                            AboutUs = objNail.AboutUs,
                            AboutUsHome = objNail.AboutUsHome,
                            Address = objNail.Address,
                            BusinessHours = objNail.BusinessHours,
                            Coupons = objNail.Coupons,
                            Domain = objNail.Domain,
                            GooglePlus = objNail.GooglePlus,
                            HyperLinkTell = objNail.HyperLinkTell,
                            LinkBookingAppointment = objNail.LinkBookingAppointment,
                            LinkGoogleMapAddress = objNail.LinkGoogleMapAddress,
                            LinkIFrameGoogleMap = objNail.LinkIFrameGoogleMap,
                            Logo = objNail.Logo,
                            Name = objNail.Name,
                            TextTell = objNail.TextTell,
                            DateTimeCreate = objDetail.DateTimeCreate
                        });
                    case "NAIL_PRICES":
                        var objNailPrices = JsonConvert.DeserializeObject<NailPrices>(objDetail.DataJson);
                        return PartialView("DataDetail_Prices", new NailPrices
                        {
                            Position = objNailPrices.Position,
                            URL = objNailPrices.URL,
                            Status = objNailPrices.Status,
                            DateTimeCreate = objDetail.DateTimeCreate
                        });
                    case "NAIL_COUPON":
                        var objNailCoupon = JsonConvert.DeserializeObject<NailCoupon>(objDetail.DataJson);
                        return PartialView("DataDetail_Coupon", new NailCoupon
                        {
                            Position = objNailCoupon.Position,
                            URL = objNailCoupon.URL,
                            Status = objNailCoupon.Status,
                            DateTimeCreate = objDetail.DateTimeCreate
                        });
                    case "NAIL_SOCIAL":
                        var objNailSocial = JsonConvert.DeserializeObject<NailSocial>(objDetail.DataJson);
                        return PartialView("DataDetail_Social", new NailSocial
                        {
                            Position = objNailSocial.Position,
                            URL = objNailSocial.URL,
                            Status = objNailSocial.Status,
                            DateTimeCreate = objDetail.DateTimeCreate
                        });
                }
                return Content("No view");
            }    
        }
    }
}