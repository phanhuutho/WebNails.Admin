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
    public class SocialController : Controller
    {
        private INailSocialRepository _nailSocialRepository;
        private INailRepository _nailRepository;
        private ISocialRepository _socialRepository;
        public SocialController(INailSocialRepository nailSocialRepository, INailRepository nailRepository, ISocialRepository socialRepository)
        {
            _nailSocialRepository = nailSocialRepository;
            _nailRepository = nailRepository;
            _socialRepository = socialRepository;
        }
        // GET: Social
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

                _nailSocialRepository.InitConnection(sqlConnect);
                _nailRepository.InitConnection(sqlConnect);

                var objResult = _nailSocialRepository.GetNailSocial(param);

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
            if (Session["Cur_NailID"] == null || Session["Cur_Domain"] == null || Session["Cur_NailName"] == null)
            {
                return RedirectToAction("Index", "Nail");
            }
            if (ID == 0)
            {

                using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
                {
                    _socialRepository.InitConnection(sqlConnect);

                    ViewBag.Socials = _socialRepository.GetSocials((int)Session["Cur_NailID"]);
                    return View(new NailSocial() { ID = 0, Nail_ID = (int)Session["Cur_NailID"] });
                }
            }
            else
            {
                using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
                {
                    _nailSocialRepository.InitConnection(sqlConnect);
                    _socialRepository.InitConnection(sqlConnect);

                    var objNailSocial = _nailSocialRepository.GetNailSocialByID(ID);

                    ViewBag.Socials = _socialRepository.GetSocials(objNailSocial.Nail_ID);
                    return View(objNailSocial);
                }
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Credit(NailSocial item)
        {
            if (Session["Cur_NailID"] == null || Session["Cur_Domain"] == null || Session["Cur_NailName"] == null)
            {
                return RedirectToAction("Index", "Nail");
            }
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                _nailSocialRepository.InitConnection(sqlConnect);
                var intCount = _nailSocialRepository.SaveChange(item);
                if (intCount == 1)
                {
                    return Json($"{(item.ID == 0 ? "Thêm" : "Sửa")} thông tin Social - " + Session["Cur_NailName"] + " thành công", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json($"{(item.ID == 0 ? "Thêm" : "Sửa")} thông tin Social - " + Session["Cur_NailName"] + " thất bại", JsonRequestBehavior.AllowGet);
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
                    _nailSocialRepository.InitConnection(sqlConnect);
                    var intCount = _nailSocialRepository.DeleteNailSocial(ID);
                    if (intCount == 1)
                    {
                        return Json("Xóa thành công Social", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Xóa thất bại Social", JsonRequestBehavior.AllowGet);
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

                _nailSocialRepository.InitConnection(sqlConnect);

                var objResult = _nailSocialRepository.GetNailSocial(param);

                ViewBag.Count = param.Get<int>("@intTotalRecord");

                return PartialView(objResult);
            }
        }
    }
}