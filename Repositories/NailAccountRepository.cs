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
    public class NailAccountRepository : INailAccountRepository
    {
        protected SqlConnection _sqlConnection { get; set; }

        public void InitConnection(SqlConnection sqlConnection)
        {
            this._sqlConnection = sqlConnection;
        }

        public NailAccount GetNailAccount(int ID)
        {
            var objAccount = this._sqlConnection.Query<NailAccount>(@"spNailAccount_GetByID", new { intID = ID }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(new NailAccount()).FirstOrDefault();
            return objAccount;
        }

        public NailAccount GetNailAccount(string Username, string Password)
        {
            var objAccount = this._sqlConnection.Query<NailAccount>(@"spNailAccount_GetByUsernameAndPassword", new { strUsername = Username, strPassword = Password }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(new NailAccount()).FirstOrDefault();
            return objAccount;
        }

        public NailAccount GetNailAccount(string Email)
        {
            var objAccount = this._sqlConnection.Query<NailAccount>(@"spNailAccount_GetByEmail", new { strEmail = Email }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(new NailAccount()).FirstOrDefault();
            return objAccount;
        }
    }
}