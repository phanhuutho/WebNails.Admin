using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebNails.Admin.Interfaces;
using WebNails.Admin.Models;
using WebNails.Admin.Utilities;

namespace WebNails.Admin.Controllers
{
    public class NailController : Controller
    {
        private readonly string TokenKeyAPI = ConfigurationManager.AppSettings["TokenKeyAPI"];
        private readonly string SaltKeyAPI = ConfigurationManager.AppSettings["SaltKeyAPI"];
        private readonly string VectorKeyAPI = ConfigurationManager.AppSettings["VectorKeyAPI"];
        private INailRepository _nailRepository;
        private INailCouponRepository _nailCouponRepository;
        private INailPricesRepository _nailPricesRepository;
        private ISocialRepository _socialRepository;
        private IActionDetailRepository _actionDetailRepository;
        private INailAccountRepository _nailAccountRepository;
        private INailApiRepository _nailApiRepository;
        public NailController(INailRepository nailRepository, INailCouponRepository nailCouponRepository, INailPricesRepository nailPricesRepository, ISocialRepository socialRepository, IActionDetailRepository actionDetailRepository, INailAccountRepository nailAccountRepository, INailApiRepository nailApiRepository)
        {
            _nailRepository = nailRepository;
            _nailCouponRepository = nailCouponRepository;
            _nailPricesRepository = nailPricesRepository;
            _socialRepository = socialRepository;
            _actionDetailRepository = actionDetailRepository;
            _nailAccountRepository = nailAccountRepository;
            _nailApiRepository = nailApiRepository;
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
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailApiRepository.InitConnection(sqlConnect);
                if (ID == 0)
                {
                    Session.Add("Cur_Domain", "Temp");
                    return View(new Nail() { ID = 0, Name = "", GUID = Guid.NewGuid(), NailApis = _nailApiRepository.GetNails().Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Url }).ToList(), FeePaypal = 0, SalesOff = 0, IsBuyerFeePaypal = false });
                }
                else
                {
                    _nailRepository.InitConnection(sqlConnect);
                    var objNail = _nailRepository.GetNailByID(ID);

                    if (objNail != null && objNail.NailApi_ID != null && objNail.NailApi_ID > 0)
                    {
                        var objNailApi = _nailApiRepository.GetNailApiByID(objNail.NailApi_ID ?? 0);
                        if (!string.IsNullOrEmpty(objNailApi.Url) && objNailApi.Token != null)
                        {
                            var Token = new { Token = objNailApi.Token, Domain = objNail.Domain, TimeExpire = DateTime.Now.AddMinutes(5) };
                            var jsonStringToken = JsonConvert.SerializeObject(Token);
                            var strEncrypt = Sercurity.EncryptToBase64(jsonStringToken, TokenKeyAPI, SaltKeyAPI, VectorKeyAPI);
                            return Redirect(string.Format("{0}/Nail/Credit/{1}?token={2}&Cur_NailID={3}&Username={4}", objNailApi.Url, ID, strEncrypt, objNail.ID, User.Identity.Name));
                        }
                    }
                    objNail.NailApis = _nailApiRepository.GetNails().Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Url, Selected = x.ID == (objNail.NailApi_ID ?? 0) }).ToList();
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
                if (intCount > 0)
                {
                    var isCreate = item.ID == 0;
                    item.ID = isCreate ? intCount : item.ID;
                    _nailAccountRepository.InitConnection(sqlConnect);
                    var objAccount = _nailAccountRepository.GetNailAccountByUsername(User.Identity.Name);

                    _actionDetailRepository.InitConnection(sqlConnect);
                    _actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "NAIL", UserID = objAccount.ID, Description = $"{(isCreate ? "Thêm" : "Sửa")} thông tin " + item.Name, DataJson = JsonConvert.SerializeObject(item), Field = "ID", FieldValue = item.ID });

                    return Json($"{(isCreate ? "Thêm" : "Sửa")} thông tin " + item.Name + " thành công", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json($"{(item.ID == 0 ? "Thêm" : "Sửa")} thông tin " + item.Name + " thất bại", JsonRequestBehavior.AllowGet);
                }
            }    
        }

        [Token]
        public ActionResult ApiCredit(int ID, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Content("Invalid Token");
            }
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailApiRepository.InitConnection(sqlConnect);
                _nailRepository.InitConnection(sqlConnect);
                var objNail = _nailRepository.GetNailByID(ID);
                objNail.NailApis = _nailApiRepository.GetNails().Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Url, Selected = x.ID == (objNail.NailApi_ID ?? 0) }).ToList();
                return Json(objNail, JsonRequestBehavior.AllowGet);
            }
        }

        [Token]
        [HttpPost]
        public ActionResult ApiCredit(Nail item, string Username, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Content("Invalid Token");
            }
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailRepository.InitConnection(sqlConnect);
                var intCount = _nailRepository.SaveChange(item);
                if (intCount > 0)
                {
                    var isCreate = item.ID == 0;
                    item.ID = isCreate ? intCount : item.ID;
                    _nailAccountRepository.InitConnection(sqlConnect);
                    var objAccount = _nailAccountRepository.GetNailAccountByUsername(Username);

                    _actionDetailRepository.InitConnection(sqlConnect);
                    _actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "NAIL", UserID = objAccount.ID, Description = $"{(isCreate ? "Thêm" : "Sửa")} thông tin " + item.Name, DataJson = JsonConvert.SerializeObject(item), Field = "ID", FieldValue = item.ID });

                    return Json(1);
                }
                else
                {
                    return Json(0);
                }
            }
        }


        [Authorize]
        public async Task<ActionResult> SyncDataWeb(int ID)
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
                    var objNailCoupon = _nailCouponRepository.GetNailCouponsByNailID(ID).Where(x => x.Status).OrderBy(x => x.Position).Select(x => new JsonCoupon { Src = x.URL, Status = x.Status }).ToList();

                    _nailPricesRepository.InitConnection(sqlConnect);
                    var objNailPrices = _nailPricesRepository.GetNailPricesByNailID(ID).Where(x => x.Status).OrderBy(x => x.Position).Select(x => new JsonPrice { Src = x.URL, Status = x.Status }).ToList();

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
                        Telegram = objNailSocial.Where(x => x.Title == "Telegram").DefaultIfEmpty(new Social()).Select(x => new JsonSocial { BackgroundColor = x.BackgroundColor, ClassIcon = x.ClassIcon, Title = x.Title, Position = x.Position, Url = x.URL ?? "" }).FirstOrDefault(),
                        Facebook = objNailSocial.Where(x => x.Title == "Facebook").DefaultIfEmpty(new Social()).Select(x => new JsonSocial { BackgroundColor = x.BackgroundColor, ClassIcon = x.ClassIcon, Title = x.Title, Position = x.Position, Url = x.URL ?? "" }).FirstOrDefault(),
                        Instagram = objNailSocial.Where(x => x.Title == "Instagram").DefaultIfEmpty(new Social()).Select(x => new JsonSocial { BackgroundColor = x.BackgroundColor, ClassIcon = x.ClassIcon, Title = x.Title, Position = x.Position, Url = x.URL ?? "" }).FirstOrDefault(),
                        Twitter = objNailSocial.Where(x => x.Title == "Twitter").DefaultIfEmpty(new Social()).Select(x => new JsonSocial { BackgroundColor = x.BackgroundColor, ClassIcon = x.ClassIcon, Title = x.Title, Position = x.Position, Url = x.URL ?? "" }).FirstOrDefault(),
                        Youtube = objNailSocial.Where(x => x.Title == "Youtube").DefaultIfEmpty(new Social()).Select(x => new JsonSocial { BackgroundColor = x.BackgroundColor, ClassIcon = x.ClassIcon, Title = x.Title, Position = x.Position, Url = x.URL ?? "" }).FirstOrDefault(),
                        Token = "", 
                        SalesOff = objNail.SalesOff,
                        FeePaypal = objNail.FeePaypal,
                        IsBuyerFeePaypal = objNail.IsBuyerFeePaypal
                    };
                    
                    if (objNail.NailApi_ID != null && objNail.NailApi_ID > 0)
                    {
                        _nailApiRepository.InitConnection(sqlConnect);
                        var ApiDomain = _nailApiRepository.GetNailApiByID(objNail.NailApi_ID ?? 0);
                        if (ApiDomain != null && ApiDomain.Token != null && !string.IsNullOrEmpty(ApiDomain.Url))
                        {
                            jsonInfo.Token = ApiDomain.Token.ToString();

                            var Token = new { Token = jsonInfo.Token, Domain = objNail.Domain, TimeExpire = DateTime.Now.AddMinutes(5) };
                            var jsonStringToken = JsonConvert.SerializeObject(Token);
                            var strEncrypt = Sercurity.EncryptToBase64(jsonStringToken, TokenKeyAPI, SaltKeyAPI, VectorKeyAPI);

                            var urlSendData = string.Format("{0}/Nail/SyncDataWeb?token={1}&domain={2}", ApiDomain.Url, strEncrypt, objNail.Domain);
                            var dataJson = new
                            {
                                jsonInfo,
                                txtBusinessHours = objNail.BusinessHours,
                                txtAboutUs = objNail.AboutUs,
                                txtAboutUsHome = objNail.AboutUsHome
                            };
                            var result = await PostStringJsonFromURL(urlSendData, JsonConvert.SerializeObject(dataJson));
                        }
                    }   
                    else
                    {
                        Commons.GenerateDataWeb(jsonInfo, objNail.BusinessHours, objNail.AboutUs, objNail.AboutUsHome, objNail.Domain, ConfigurationManager.AppSettings["VirtualData"]);
                    }    

                    _nailAccountRepository.InitConnection(sqlConnect);
                    var objAccount = _nailAccountRepository.GetNailAccountByUsername(User.Identity.Name);

                    _actionDetailRepository.InitConnection(sqlConnect);
                    //_actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "SyncDataWeb", UserID = objAccount.ID, Description = $"Cập nhật dữ liệu lên website", DataJson = JsonConvert.SerializeObject(jsonInfo) });
                    //_actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "SyncDataWeb", UserID = objAccount.ID, Description = $"Cập nhật dữ liệu lên website", DataJson = JsonConvert.SerializeObject(new { BusinessHours = objNail.BusinessHours }) });
                    //_actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "SyncDataWeb", UserID = objAccount.ID, Description = $"Cập nhật dữ liệu lên website", DataJson = JsonConvert.SerializeObject(new { AboutUs = objNail.AboutUs }) });
                    //_actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "SyncDataWeb", UserID = objAccount.ID, Description = $"Cập nhật dữ liệu lên website", DataJson = JsonConvert.SerializeObject(new { AboutUsHome = objNail.AboutUsHome }) });
                    _actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "SyncDataWeb", UserID = objAccount.ID, Description = $"Cập nhật dữ liệu lên website", DataJson = JsonConvert.SerializeObject(new { ID = objNail.ID }), Field = "ID", FieldValue = objNail.ID });

                    return Json("Cập nhật dữ liệu lên website thành công !", JsonRequestBehavior.AllowGet);
                }
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult CheckSecurityPassword(string strPassword)
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                var strMD5Password = Sercurity.Md5(strPassword);
                _nailAccountRepository.InitConnection(sqlConnect);
                var objAccount = _nailAccountRepository.GetNailAccount(User.Identity.Name, strMD5Password);
                if (objAccount.ID != 0)
                {
                    return Json(new { CheckPassword = true, Message = "Success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CheckPassword = false, Message = "Mật khẩu không chính xác" }, JsonRequestBehavior.AllowGet);
                }
            }    
        }

        private async Task<string> PostStringJsonFromURL(string url, string dataJson)
        {
            var requestContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(url, requestContent);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                return "";
            }
        }
    }
}