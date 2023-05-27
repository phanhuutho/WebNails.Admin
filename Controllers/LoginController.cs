using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebNails.Admin.Interfaces;
using WebNails.Admin.Models;
using WebNails.Admin.Utilities;

namespace WebNails.Admin.Controllers
{
    public class LoginController : Controller
    {
        private INailAccountRepository _nailAccountRepository;
        public LoginController(INailAccountRepository nailAccountRepository)
        {
            _nailAccountRepository = nailAccountRepository;
        }
        // GET: administrator/Login
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginModel model)
        {

            var strMD5Password = Sercurity.Md5(model.Password);
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                var queryString = Request.UrlReferrer.Query;

                var queryDictionary = HttpUtility.ParseQueryString(queryString);

                _nailAccountRepository.InitConnection(sqlConnect);

                var objAccount = _nailAccountRepository.GetNailAccount(model.Username, strMD5Password);
                if (objAccount.ID != 0)
                {
                    var ValidationCode = Commons.RandomCodeNumber(6);
                    Session["User_" + model.Username + "_ValidationCode"] = ValidationCode;
                    Session["User_" + model.Username + "_Email"] = objAccount.Email;
                    Session["User_" + model.Username + "_Role"] = objAccount.Role;
                    MailHelper.SendMail(objAccount.Email, "Verify Code Login", "Access Code Login: " + ValidationCode);
                    if (queryDictionary.Count > 0)
                    {
                        return Json(new { ReturnUrl = queryDictionary.Get("ReturnUrl"), IsLogin = true, Message = "", Username = model.Username, ValidationCode = Sercurity.Md5(ValidationCode) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { ReturnUrl = "/ControlPanel/Index", IsLogin = true, Message = "", Username = model.Username, ValidationCode = Sercurity.Md5(ValidationCode) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { ReturnUrl = queryDictionary.Get("ReturnUrl"), IsLogin = false, Message = "Thông tin đăng nhập không chính xác" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [AllowAnonymous]
        public ActionResult ConfirmLoginByCode(string Username, string ValidationCode)
        {
            return View(new LoginModel() { Username = Username, ValidationCode = ValidationCode });
        }

        [HttpPost]
        public ActionResult ConfirmLoginByCode(LoginModel model)
        {
            var queryString = Request.UrlReferrer.Query;

            var queryDictionary = HttpUtility.ParseQueryString(queryString);

            if (Session["User_" + model.Username + "_ValidationCode"] != null)
            {
                var Md5Code = Sercurity.Md5(model.Code);
                var SessionValidationCode = string.Format("{0}", Session["User_" + model.Username + "_ValidationCode"]);
                if (Md5Code == model.ValidationCode && model.Code == SessionValidationCode)
                {
                    var SessionRole = string.Format("{0}", Session["User_" + model.Username + "_Role"]);
                    var ticket = new FormsAuthenticationTicket(1, model.Username, System.DateTime.Now, System.DateTime.Now.AddHours(1), true, SessionRole ?? "", FormsAuthentication.FormsCookiePath);
                    var strEncrypt = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, strEncrypt);
                    Response.Cookies.Add(cookie);

                    Session.Remove("User_" + model.Username + "_ValidationCode");
                    Session.Remove("User_" + model.Username + "_Email");
                    Session.Remove("User_" + model.Username + "_Role");
                    if (queryDictionary.Count > 0)
                    {
                        return Json(new { ReturnUrl = queryDictionary.Get("ReturnUrl"), IsLogin = true, Message = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { ReturnUrl = "/ControlPanel/Index", IsLogin = true, Message = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { ReturnUrl = queryDictionary.Get("ReturnUrl"), IsLogin = false, Message = "Mã xác nhận không chính xác" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { ReturnUrl = queryDictionary.Get("ReturnUrl"), IsLogin = false, Message = "Mã xác nhận đã quá hạn." }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                cookie.Expires = System.DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Index", "Login");
        }
    }
}