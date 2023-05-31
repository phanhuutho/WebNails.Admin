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
    public class NailPricesRepository : INailPricesRepository
    {
        protected SqlConnection _sqlConnection { get; set; }
        public int DeleteNailPrices(int ID)
        {
            var intCount = _sqlConnection.Execute(@"spNailPrices_DeleteByID", new { intID = ID }, commandType: CommandType.StoredProcedure);
            return intCount;
        }

        public NailPrices GetNailPricesByID(int ID)
        {
            var objNailPrices = this._sqlConnection.Query<NailPrices>(@"spNailPrices_GetNailPricesByID", new { intID = ID }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(new NailPrices()).FirstOrDefault();
            return objNailPrices;
        }

        public IEnumerable<NailPrices> GetNailPrices(DynamicParameters param)
        {
            var objNailPrices = this._sqlConnection.Query<NailPrices>(@"spNailPrices_GetNailPrices", param, commandType: CommandType.StoredProcedure);
            return objNailPrices;
        }

        public IEnumerable<NailPrices> GetNailPricesByNailID(int NailID)
        {
            var objNailPrices = this._sqlConnection.Query<NailPrices>(@"spNailPrices_GetNailPricesByNailID", new { intNailID = NailID }, commandType: CommandType.StoredProcedure);
            return objNailPrices;
        }

        public void InitConnection(SqlConnection sqlConnection)
        {
            this._sqlConnection = sqlConnection;
        }

        public int SaveChange(NailPrices item)
        {
            var intCount = _sqlConnection.Execute(@"spNailPrices_Credit",
                    new
                    {
                        intID = item.ID,
                        intNail_ID = item.Nail_ID,
                        intPosition = item.Position,
                        strURL = item.URL,
                        bitStatus = item.Status
                    }, commandType: CommandType.StoredProcedure);
            return intCount;
        }
    }
}