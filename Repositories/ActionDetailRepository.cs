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
    public class ActionDetailRepository : IActionDetailRepository
    {
        protected SqlConnection _sqlConnection { get; set; }

        public int ActionDetailLog(ActionDetail log)
        {
            var intCount = _sqlConnection.Execute(@"spActionDetail_Log",
                    new
                    {
                        strTable = log.Table,
                        intUserID = log.UserID,
                        strDataJson = log.DataJson,
                        strDescription = log.Description
                    }, commandType: CommandType.StoredProcedure);
            return intCount;
        }

        public void InitConnection(SqlConnection sqlConnection)
        {
            this._sqlConnection = sqlConnection;
        }
    }
}