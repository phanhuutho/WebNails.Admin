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
    public class NailSocialRepository : INailSocialRepository
    {
        protected SqlConnection _sqlConnection { get; set; }

        public int DeleteNailSocial(int ID)
        {
            var intCount = _sqlConnection.Execute(@"spNailSocial_DeleteByID", new { intID = ID }, commandType: CommandType.StoredProcedure);
            return intCount;
        }

        public IEnumerable<NailSocial> GetNailSocial(DynamicParameters param)
        {
            var objNailPrices = this._sqlConnection.Query<NailSocial>(@"spNailSocial_GetNailSocial", param, commandType: CommandType.StoredProcedure);
            return objNailPrices;
        }

        public NailSocial GetNailSocialByID(int ID)
        {
            var objNailSocial = this._sqlConnection.Query<NailSocial>(@"spNailSocial_GetNailSocialByID", new { intID = ID }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(new NailSocial()).FirstOrDefault();
            return objNailSocial;
        }

        public IEnumerable<NailSocial> GetNailSocialByNailID(int NailID)
        {
            var objNailSocial = this._sqlConnection.Query<NailSocial>(@"spNailSocial_GetNailSocialByNailID", new { intNailID = NailID }, commandType: CommandType.StoredProcedure);
            return objNailSocial;
        }

        public void InitConnection(SqlConnection sqlConnection)
        {
            this._sqlConnection = sqlConnection;
        }

        public int SaveChange(NailSocial item)
        {
            var intID = _sqlConnection.Query<int>(@"spNailSocial_Credit",
                    new
                    {
                        intID = item.ID,
                        intNail_ID = item.Nail_ID,
                        intSocial_ID = item.Social_ID,
                        intPosition = item.Position,
                        strURL = item.URL,
                        bitStatus = item.Status
                    }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(0).FirstOrDefault();
            return intID;
        }
    }
}