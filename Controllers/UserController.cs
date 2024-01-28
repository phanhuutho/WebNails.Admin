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
    public class UserController : Controller
    {
        private IUserSiteRepository _userSiteRepository;
        private INailRepository _nailRepository;
        public UserController(IUserSiteRepository userSiteRepository, INailRepository nailRepository)
        {
            this._userSiteRepository = userSiteRepository;
            this._nailRepository = nailRepository;
        }
        // GET: User
        [Authorize]
        public ActionResult Index(string Search = "", int Nail_ID = 0)
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                var intSkip = Utilities.PagingHelper.Skip;

                var param = new DynamicParameters();
                param.Add("@intSkip", intSkip);
                param.Add("@intTake", Utilities.PagingHelper.CountSort);
                param.Add("@strValue", Search);
                param.Add("@intNailID", Nail_ID);
                param.Add("@intTotalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);

                _userSiteRepository.InitConnection(sqlConnect);

                var objResult = _userSiteRepository.GetUserSites(param);

                ViewBag.Count = param.Get<int>("@intTotalRecord");

                return View(objResult);
            }
        }

        [Authorize]
        public ActionResult Credit(int ID = 0, int Nail_ID = 0)
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailRepository.InitConnection(sqlConnect);
                if (ID == 0)
                {
                    Random r = new Random();
                    int rInt = r.Next(0, 1000000);
                    return View(new UserSite() { ID = 0, Nail_ID = Nail_ID, Password = rInt.ToString(), Nails = _nailRepository.GetNails().Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Domain }).ToList() });
                }
                else
                {
                    _userSiteRepository.InitConnection(sqlConnect);
                    var objUserSite = _userSiteRepository.GetUserSiteByID(ID);
                    objUserSite.Nails = _nailRepository.GetNails().Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Domain, Selected = x.ID == objUserSite.Nail_ID }).ToList();
                    return View(objUserSite);
                }
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Credit(UserSite item)
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _userSiteRepository.InitConnection(sqlConnect);
                var intCount = _userSiteRepository.SaveChange(item);
                if (intCount > 0)
                {
                    var isCreate = item.ID == 0;
                    item.ID = isCreate ? intCount : item.ID;
                    return Json($"{(isCreate ? "Thêm" : "Sửa")} thông tin User thành công", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json($"{(item.ID == 0 ? "Thêm" : "Sửa")} thông tin User thất bại", JsonRequestBehavior.AllowGet);
                }
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(int ID = 0)
        {
            if (ID == 0)
            {
                return RedirectToAction("Index", "ControlPanel");
            }
            else
            {
                using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
                {
                    _userSiteRepository.InitConnection(sqlConnect);
                    var intCount = _userSiteRepository.DeleteUserSiteByID(ID);
                    if (intCount == 1)
                    {
                        return Json("Xóa thành công User", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Xóa thất bại User", JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        [Authorize]
        public ActionResult GetTable(string Search = "", int Nail_ID = 0)
        {
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                var intSkip = Utilities.PagingHelper.Skip;

                var param = new DynamicParameters();
                param.Add("@intSkip", intSkip);
                param.Add("@intTake", Utilities.PagingHelper.CountSort);
                param.Add("@strValue", Search);
                param.Add("@intNail_ID", Nail_ID);
                param.Add("@intTotalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);

                _userSiteRepository.InitConnection(sqlConnect);

                var objResult = _userSiteRepository.GetUserSites(param);

                ViewBag.Count = param.Get<int>("@intTotalRecord");

                return PartialView(objResult);
            }
        }
    }
}