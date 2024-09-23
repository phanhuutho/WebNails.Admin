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
    public class PricesController : Controller
    {
        private readonly string TokenKeyAPI = ConfigurationManager.AppSettings["TokenKeyAPI"];
        private readonly string SaltKeyAPI = ConfigurationManager.AppSettings["SaltKeyAPI"];
        private readonly string VectorKeyAPI = ConfigurationManager.AppSettings["VectorKeyAPI"];
        private INailPricesRepository _nailPricesRepository;
        private INailRepository _nailRepository;
        private IActionDetailRepository _actionDetailRepository;
        private INailAccountRepository _nailAccountRepository;
        private INailApiRepository _nailApiRepository;
        public PricesController(INailPricesRepository nailPricesRepository, INailRepository nailRepository, IActionDetailRepository actionDetailRepository, INailAccountRepository nailAccountRepository, INailApiRepository nailApiRepository)
        {
            _nailPricesRepository = nailPricesRepository;
            _nailRepository = nailRepository;
            _actionDetailRepository = actionDetailRepository;
            _nailAccountRepository = nailAccountRepository;
            _nailApiRepository = nailApiRepository;
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

                var objResult = _nailPricesRepository.GetNailPrices(param);

                ViewBag.Count = param.Get<int>("@intTotalRecord");

                //get info nail
                var objNail = _nailRepository.GetNailByID(Nail_ID);
                Session.Add("Cur_Domain", objNail.Domain);
                Session.Add("Cur_NailName", objNail.Name);
                Session.Add("Cur_NailID", objNail.ID);

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
                        var strEncrypt = Sercurity.EncryptToBase64(jsonStringToken, TokenKeyAPI, SaltKeyAPI, VectorKeyAPI);
                        return Redirect(string.Format("{0}/Prices/Credit/{1}?token={2}&Cur_NailID={3}&Username={4}", objNailApi.Url, ID, strEncrypt, intNailID, User.Identity.Name));
                    }
                }

                if (ID == 0)
                {
                    return View(new NailPrices() { ID = 0, Nail_ID = (int)Session["Cur_NailID"] });
                }
                else
                {
                    _nailPricesRepository.InitConnection(sqlConnect);
                    var objNailPrices = _nailPricesRepository.GetNailPricesByID(ID);
                    return View(objNailPrices);
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
                if (intCount > 0)
                {
                    var isCreate = item.ID == 0;
                    item.ID = isCreate ? intCount : item.ID;
                    _nailAccountRepository.InitConnection(sqlConnect);
                    var objAccount = _nailAccountRepository.GetNailAccountByUsername(User.Identity.Name);

                    _actionDetailRepository.InitConnection(sqlConnect);
                    _actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "NAIL_PRICES", UserID = objAccount.ID, Description = $"{(isCreate ? "Thêm" : "Sửa")} thông tin Prices List - " + Session["Cur_NailName"], DataJson = JsonConvert.SerializeObject(item), Field = "Nail_ID", FieldValue = item.Nail_ID });
                    
                    return Json($"{(isCreate ? "Thêm" : "Sửa")} thông tin Prices List - " + Session["Cur_NailName"] + " thành công", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json($"{(item.ID == 0 ? "Thêm" : "Sửa")} thông tin Prices List - " + Session["Cur_NailName"] + " thất bại", JsonRequestBehavior.AllowGet);
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
                _nailPricesRepository.InitConnection(sqlConnect);
                var objNailPrices = _nailPricesRepository.GetNailPricesByID(ID);
                return Json(objNailPrices, JsonRequestBehavior.AllowGet);
            }
        }

        [Token]
        [HttpPost]
        public ActionResult ApiCredit(NailPrices item, string Cur_NailName, string Username, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Content("Invalid Token");
            }
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailPricesRepository.InitConnection(sqlConnect);
                var intCount = _nailPricesRepository.SaveChange(item);
                if (intCount > 0)
                {
                    var isCreate = item.ID == 0;
                    item.ID = isCreate ? intCount : item.ID;
                    _nailAccountRepository.InitConnection(sqlConnect);
                    var objAccount = _nailAccountRepository.GetNailAccountByUsername(Username);

                    _actionDetailRepository.InitConnection(sqlConnect);
                    _actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "NAIL_PRICES", UserID = objAccount.ID, Description = $"{(isCreate ? "Thêm" : "Sửa")} thông tin Prices List - " + Cur_NailName, DataJson = JsonConvert.SerializeObject(item), Field = "Nail_ID", FieldValue = item.Nail_ID });

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
                    _nailPricesRepository.InitConnection(sqlConnect);
                    var item = _nailPricesRepository.GetNailPricesByID(ID);
                    var intCount = _nailPricesRepository.DeleteNailPrices(ID);
                    if (intCount == 1)
                    {
                        _nailAccountRepository.InitConnection(sqlConnect);
                        var objAccount = _nailAccountRepository.GetNailAccountByUsername(User.Identity.Name);

                        _actionDetailRepository.InitConnection(sqlConnect);
                        _actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "NAIL_PRICES", UserID = objAccount.ID, Description = "Xóa Prices List - " + Session["Cur_NailName"], DataJson = JsonConvert.SerializeObject(item), Field = "Nail_ID", FieldValue = item.Nail_ID });

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

                var objResult = _nailPricesRepository.GetNailPrices(param);

                ViewBag.Count = param.Get<int>("@intTotalRecord");

                return PartialView(objResult);
            }
        }
    }
}