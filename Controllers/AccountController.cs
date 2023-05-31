using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebNails.Admin.Interfaces;
using WebNails.Admin.Models;
using WebNails.Admin.Utilities;

namespace WebNails.Admin.Controllers
{
    public class AccountController : Controller
    {
        private INailAccountRepository _nailAccountRepository;
        private IActionDetailRepository _actionDetailRepository;
        public AccountController(INailAccountRepository nailAccountRepository, IActionDetailRepository actionDetailRepository)
        {
            _nailAccountRepository = nailAccountRepository;
            _actionDetailRepository = actionDetailRepository;
        }
        // GET: Account
        [Authorize]
        public ActionResult MyAccount()
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailAccountRepository.InitConnection(sqlConnect);

                var objAccount = _nailAccountRepository.GetNailAccountByUsername(User.Identity.Name);
                return View(objAccount);
            }
        }

        [Authorize]
        public ActionResult UpdatePassword(FormCollection frmData)
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                var strUsername = User.Identity.Name;
                var strPasswordOld = string.IsNullOrEmpty(frmData["password_old"]) ? "" : Sercurity.Md5(frmData["password_old"]);
                var strPasswordNew = string.IsNullOrEmpty(frmData["password_new"]) ? "" : Sercurity.Md5(frmData["password_new"]);

                _nailAccountRepository.InitConnection(sqlConnect);

                var objAccount = _nailAccountRepository.GetNailAccount(strUsername, strPasswordOld);
                if (objAccount != null)
                {
                    var intCount = _nailAccountRepository.UpdatePassword(new NailAccount { ID = objAccount.ID, Password = strPasswordNew });
                    if (intCount == 1)
                    {
                        _actionDetailRepository.InitConnection(sqlConnect);
                        _actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "NAIL_ACCOUNT", UserID = objAccount.ID, Description = "Đổi mật khẩu", DataJson = "{ID:" + objAccount.ID + "}" });

                        return Json("Đổi mật khẩu thành công", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Đổi mật khẩu thất bại", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("Đổi mật khẩu thất bại", JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public ActionResult UpdateMyAccount(NailAccount model)
        {
            var strUsername = User.Identity.Name;
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailAccountRepository.InitConnection(sqlConnect);
                var objAccount = _nailAccountRepository.GetNailAccountByUsername(strUsername);
                if (objAccount != null)
                {
                    var intCount = _nailAccountRepository.UpdateNailAccount(new NailAccount { ID = objAccount.ID, Fullname = model.Fullname, Phone = model.Phone });
                    if (intCount == 1)
                    {
                        _actionDetailRepository.InitConnection(sqlConnect);
                        _actionDetailRepository.ActionDetailLog(new ActionDetail { Table = "NAIL_ACCOUNT", UserID = objAccount.ID, Description = "Cập nhật thông tin account", DataJson = "{ID:" + objAccount.ID + ", Fullname:" + model.Fullname + ", Phone:" + model.Phone + "}" });

                        return Json("Cập nhật thông tin thành công", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Cập nhật thông tin thất bại", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("Cập nhật thông tin thất bại", JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}