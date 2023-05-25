using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNails.Admin.Controllers
{
    public class ControlPanelController : Controller
    {
        // GET: administrator/ControlPanel
        public ActionResult Index()
        {
            return View();
        }
    }
}