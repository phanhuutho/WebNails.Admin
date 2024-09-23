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
    public class NailApiRepository : INailApiRepository
    {
        protected SqlConnection _sqlConnection { get; set; }

        public void InitConnection(SqlConnection sqlConnection)
        {
            this._sqlConnection = sqlConnection;
        }

        public NailApi GetNailApiByID(int ID)
        {
            var objNailApi = this._sqlConnection.Query<NailApi>(@"spNailApi_GetNailApiByID", new { intID = ID }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(new NailApi()).FirstOrDefault();
            return objNailApi;
        }

        public NailApi GetNailApiByToken(Guid Token)
        {
            var objNailApi = this._sqlConnection.Query<NailApi>(@"spNailApi_GetNailApiByToken", new { strToken = Token }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(new NailApi()).FirstOrDefault();
            return objNailApi;
        }

        public List<NailApi> GetNails()
        {
            var objNailApis = this._sqlConnection.Query<NailApi>(@"spNailApi_GetNailApis", null, commandType: CommandType.StoredProcedure).ToList();
            return objNailApis;
        }
    }
}