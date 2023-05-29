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
    public class NailRepository : INailRepository
    {
        protected SqlConnection _sqlConnection { get; set; }

        public Nail GetNailByID(int ID)
        {
            var objNail = this._sqlConnection.Query<Nail>(@"spNail_GetNailByID", new { intID = ID }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(new Nail()).FirstOrDefault();
            return objNail;
        }

        public IEnumerable<Nail> GetNails(DynamicParameters param)
        {
            var objNails = this._sqlConnection.Query<Nail>(@"spNail_GetNails", param, commandType: CommandType.StoredProcedure);
            return objNails;
        }

        public void InitConnection(SqlConnection sqlConnection)
        {
            this._sqlConnection = sqlConnection;
        }

    }
}