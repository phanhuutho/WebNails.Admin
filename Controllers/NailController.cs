﻿using Dapper;
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
    public class NailController : Controller
    {
        private INailRepository _nailRepository;
        public NailController(INailRepository nailRepository)
        {
            _nailRepository = nailRepository;
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
    }
}