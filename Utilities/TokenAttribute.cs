using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNails.Admin.Utilities
{
    public class TokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (filterContext.HttpContext != null && !string.IsNullOrEmpty(filterContext.HttpContext.Request["token"]) && filterContext.HttpContext.Request.UrlReferrer.Authority == UrlReferrer)
            //{
            //    var strEncryptToken = filterContext.HttpContext.Request["token"];
            //    strEncryptToken = strEncryptToken.Replace(" ", "+");
            //    var strDecryptToken = Sercurity.DecryptString(TokenKeyAPI, strEncryptToken);
            //    var objToken = JsonConvert.DeserializeObject<TokenResult>(strDecryptToken);

            //    if (objToken == null || string.IsNullOrEmpty(objToken.Token) && objToken.Token != TokenServer || string.IsNullOrEmpty(objToken.Domain) || CheckDomainInServer(objToken.Domain) || objToken.TimeExpire == null || objToken.TimeExpire.Value < DateTime.Now)
            //    {
            //        filterContext.Result = new RedirectResult("/Home/Index");
            //    }
            //}
        }

        private bool CheckDomainInServer(string Domian)
        {
            var result = false;
            //var directories = Directory.GetDirectories(VirtualFolderWebs);
            //if (directories != null && directories.Length > 0)
            //{
            //    result = Array.IndexOf(directories, Domian) > -1;
            //}
            return result;
        }
    }
}