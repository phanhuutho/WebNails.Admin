﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNails.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "ControlPanel");
        }

        public ActionResult JsonEmpty()
        {
            return Json("");
        }
    }
}