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
    public class NailController : Controller
    {
        private INailRepository _nailRepository;
        private INailCouponRepository _nailCouponRepository;
        private INailPricesRepository _nailPricesRepository;
        private ISocialRepository _socialRepository;
        private IActionDetailRepository _actionDetailRepository;
        private INailAccountRepository _nailAccountRepository;
        public NailController(INailRepository nailRepository, INailCouponRepository nailCouponRepository, INailPricesRepository nailPricesRepository, ISocialRepository socialRepository, IActionDetailRepository actionDetailRepository, INailAccountRepository nailAccountRepository)
        {
            _nailRepository = nailRepository;
            _nailCouponRepository = nailCouponRepository;
            _nailPricesRepository = nailPricesRepository;
            _socialRepository = socialRepository;
            _actionDetailRepository = actionDetailRepository;
            _nailAccountRepository = nailAccountRepository;
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
                Session.Add("Cur_Domain","Temp");
                return View(new Nail() { ID = 0, Name = "", GUID = Guid.NewGuid() });
            }
            else
            {
                using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
                {
                    _nailRepository.InitConnection(sqlConnect);
                    var objNail = _nailRepository.GetNailByID(ID);
                    Session.Add("Cur_Domain", objNail.Domain);
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
                    _nailAccountRepository.InitConnection(sqlConnect);
                    var objAccount = _nailAccountRepository.GetNailAccount(User.Identity.Name);

                    _actionDetailRepository.InitConnection(sqlConnect);
                    _actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "NAIL", UserID = objAccount.ID, Description = $"{(item.ID == 0 ? "Thêm" : "Sửa")} thông tin " + item.Name, DataJson = JsonConvert.SerializeObject(item) });

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

                    _socialRepository.InitConnection(sqlConnect);
                    var objNailSocial = _socialRepository.GetSocialMappingNailSocialByNailID(ID);

                    var jsonInfo = new JsonInfo
                    {
                        Name = objNail.Name ?? "",
                        Logo = objNail.Logo ?? "",
                        HyperLinkTell = objNail.HyperLinkTell ?? "",
                        TextTell = objNail.TextTell ?? "",
                        LinkBookingAppointment = objNail.LinkBookingAppointment ?? "",
                        GooglePlus = objNail.GooglePlus ?? "",
                        Address = objNail.Address ?? "",
                        LinkGoogleMapAddress = objNail.LinkGoogleMapAddress ?? "",
                        LinkIFrameGoogleMap = objNail.LinkIFrameGoogleMap ?? "",
                        ShowCoupon = objNail.Coupons,
                        Coupons = objNailCoupon,
                        Prices = objNailPrices,
                        Telegram = objNailSocial.Where(x => x.Title == "Telegram").DefaultIfEmpty(new Social()).Select(x => new JsonSocial { BackgroundColor = x.BackgroundColor, ClassIcon = x.ClassIcon, Title = x.Title, Position = x.Position, Url = x.URL }).FirstOrDefault(),
                        Facebook = objNailSocial.Where(x => x.Title == "Facebook").DefaultIfEmpty(new Social()).Select(x => new JsonSocial { BackgroundColor = x.BackgroundColor, ClassIcon = x.ClassIcon, Title = x.Title, Position = x.Position, Url = x.URL }).FirstOrDefault(),
                        Instagram = objNailSocial.Where(x => x.Title == "Instagram").DefaultIfEmpty(new Social()).Select(x => new JsonSocial { BackgroundColor = x.BackgroundColor, ClassIcon = x.ClassIcon, Title = x.Title, Position = x.Position, Url = x.URL }).FirstOrDefault(),
                        Twitter = objNailSocial.Where(x => x.Title == "Twitter").DefaultIfEmpty(new Social()).Select(x => new JsonSocial { BackgroundColor = x.BackgroundColor, ClassIcon = x.ClassIcon, Title = x.Title, Position = x.Position, Url = x.URL }).FirstOrDefault(),
                        Youtube = objNailSocial.Where(x => x.Title == "Youtube").DefaultIfEmpty(new Social()).Select(x => new JsonSocial { BackgroundColor = x.BackgroundColor, ClassIcon = x.ClassIcon, Title = x.Title, Position = x.Position, Url = x.URL }).FirstOrDefault()
                    };
                    Commons.GenerateDataWeb(jsonInfo, objNail.BusinessHours, objNail.AboutUs, objNail.AboutUsHome, objNail.Domain, ConfigurationManager.AppSettings["VirtualData"]);

                    _nailAccountRepository.InitConnection(sqlConnect);
                    var objAccount = _nailAccountRepository.GetNailAccount(User.Identity.Name);

                    _actionDetailRepository.InitConnection(sqlConnect);
                    //_actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "SyncDataWeb", UserID = objAccount.ID, Description = $"Cập nhật dữ liệu lên website", DataJson = JsonConvert.SerializeObject(jsonInfo) });
                    //_actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "SyncDataWeb", UserID = objAccount.ID, Description = $"Cập nhật dữ liệu lên website", DataJson = JsonConvert.SerializeObject(new { BusinessHours = objNail.BusinessHours }) });
                    //_actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "SyncDataWeb", UserID = objAccount.ID, Description = $"Cập nhật dữ liệu lên website", DataJson = JsonConvert.SerializeObject(new { AboutUs = objNail.AboutUs }) });
                    //_actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "SyncDataWeb", UserID = objAccount.ID, Description = $"Cập nhật dữ liệu lên website", DataJson = JsonConvert.SerializeObject(new { AboutUsHome = objNail.AboutUsHome }) });
                    _actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "SyncDataWeb", UserID = objAccount.ID, Description = $"Cập nhật dữ liệu lên website", DataJson = JsonConvert.SerializeObject(new { ID = objNail.ID }) });

                    return Json("Cập nhật dữ liệu lên website thành công !", JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}