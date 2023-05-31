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
    public class SocialRepository : ISocialRepository
    {
        protected SqlConnection _sqlConnection { get; set; }
        public IEnumerable<Social> GetSocials()
        {
            var objSocial = this._sqlConnection.Query<Social>(@"spSocial_GetSocials", commandType: CommandType.StoredProcedure);
            return objSocial;
        }

        public IEnumerable<Social> GetSocials(int NailID)
        {
            var objSocial = this._sqlConnection.Query<Social>(@"spSocial_GetSocialsNotInNail", new { intNail_ID = NailID }, commandType: CommandType.StoredProcedure);
            return objSocial;
        }

        public void InitConnection(SqlConnection sqlConnection)
        {
            this._sqlConnection = sqlConnection;
        }
    }
}