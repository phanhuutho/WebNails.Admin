using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebNails.Admin.Interfaces;
using WebNails.Admin.Models;

namespace WebNails.Admin.Repositories
{
    public class UserSiteRepository : IUserSiteRepository
    {
        protected SqlConnection _sqlConnection { get; set; }
        public int DeleteUserSiteByID(int ID)
        {
            var intCount = _sqlConnection.Execute(@"spUserSite_DeleteByID", new { intID = ID }, commandType: CommandType.StoredProcedure);
            return intCount;
        }

        public UserSite GetUserSiteByID(int ID)
        {
            var objUserSite = this._sqlConnection.Query<UserSite>(@"spUserSite_GetUserSiteByID", new { intID = ID }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(new UserSite()).FirstOrDefault();
            return objUserSite;
        }

        public UserSite GetUserSite(string Username, string Password, int NailID)
        {
            var objAccount = this._sqlConnection.Query<UserSite>(@"spUserSite_GetByUsernameAndPassword", new { strUsername = Username, strPassword = Password, intNailID = NailID }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(new UserSite()).FirstOrDefault();
            return objAccount;
        }

        public UserSite GetUserSiteByUsername(string Username, int NailID)
        {
            var objAccount = this._sqlConnection.Query<UserSite>(@"spUserSite_GetByUsername", new { strUsername = Username, intNailID = NailID }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(new UserSite()).FirstOrDefault();
            return objAccount;
        }

        public IEnumerable<UserSite> GetUserSites(DynamicParameters param)
        {
            var objUserSite = this._sqlConnection.Query<UserSite>(@"spUserSite_GetUserSites", param, commandType: CommandType.StoredProcedure);
            return objUserSite;
        }

        public void InitConnection(SqlConnection sqlConnection)
        {
            this._sqlConnection = sqlConnection;
        }

        public int SaveChange(UserSite item)
        {
            var intID = _sqlConnection.Query<int>(@"spUserSite_Credit",
                    new
                    {
                        intID = item.ID,
                        intNail_ID = item.Nail_ID,
                        bitStatus = item.Status,
                        strUsername = item.Username,
                        strPassword = item.Password,
                        strFullname = item.Fullname
                    }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(0).FirstOrDefault();
            return intID;
        }
    }
}