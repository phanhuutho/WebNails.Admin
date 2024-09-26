using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebNails.Admin.Models;
using WebNails.Admin.Repositories;

namespace WebNails.Admin.Utilities
{
    public class TokenAttribute : ActionFilterAttribute
    {
        private readonly string TokenKeyAPI = ConfigurationManager.AppSettings["TokenKeyAPI"];
        private readonly string SaltKeyAPI = ConfigurationManager.AppSettings["SaltKeyAPI"];
        private readonly string VectorKeyAPI = ConfigurationManager.AppSettings["VectorKeyAPI"];
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext != null && !string.IsNullOrEmpty(filterContext.HttpContext.Request["token"]))
            {
                var strEncryptToken = filterContext.HttpContext.Request["token"];
                var strDecryptToken = Sercurity.DecryptFromBase64(strEncryptToken, TokenKeyAPI, SaltKeyAPI, VectorKeyAPI);
                var objToken = JsonConvert.DeserializeObject<TokenResult>(strDecryptToken);

                if (objToken == null || string.IsNullOrEmpty(objToken.Token) || !CheckTokenAPI(objToken.Token) || string.IsNullOrEmpty(objToken.Domain) || !CheckDomainInServer(objToken.Domain) || objToken.TimeExpire == null || objToken.TimeExpire.Value < DateTime.Now)
                {
                    filterContext.Result = new RedirectResult("/Home/JsonEmpty");
                }
            }
            else
            {
                filterContext.Result = new RedirectResult("/Home/JsonEmpty");
            }
        }

        private bool CheckDomainInServer(string Domain)
        {
            var result = false;
            var nailRepository = new NailRepository();
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                nailRepository.InitConnection(sqlConnect);
                var objNail = nailRepository.GetNailByDomain(Domain);
                result = objNail != null && objNail.ID > 0;
            }    
            return result;
        }

        private bool CheckTokenAPI(string TokenAPI)
        {
            var result = false;
            var nailAPIRepository = new NailApiRepository();
            using (var sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ContextDatabase"].ConnectionString))
            {
                var token = Guid.Parse(TokenAPI);
                nailAPIRepository.InitConnection(sqlConnect);
                var objNailApi = nailAPIRepository.GetNailApiByToken(token);
                result = objNailApi != null && objNailApi.ID > 0;
            }    
            return result;
        }
    }
}