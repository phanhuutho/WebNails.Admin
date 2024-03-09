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
    public class NailCodeSaleRepository : INailCodeSaleRepository
    {
        protected SqlConnection _sqlConnection { get; set; }
        public int DeleteNailCodeSale(int ID)
        {
            var intCount = _sqlConnection.Execute(@"spNailCodeSale_DeleteByID", new { intID = ID }, commandType: CommandType.StoredProcedure);
            return intCount;
        }

        public NailCodeSale GetNailCodeSaleByID(int ID)
        {
            var objNailCodeSale = this._sqlConnection.Query<NailCodeSale>(@"spNailCodeSale_GetNailCodeSaleByID", new { intID = ID }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(new NailCodeSale()).FirstOrDefault();
            return objNailCodeSale;
        }

        public IEnumerable<NailCodeSale> GetNailCodeSales(DynamicParameters param)
        {
            var objNailCodeSales = this._sqlConnection.Query<NailCodeSale>(@"spNailCodeSale_GetNailCodeSales", param, commandType: CommandType.StoredProcedure);
            return objNailCodeSales;
        }

        public void InitConnection(SqlConnection sqlConnection)
        {
            this._sqlConnection = sqlConnection;
        }

        public int SaveChange(NailCodeSale item)
        {
            var intID = _sqlConnection.Query<int>(@"spNailCodeSale_Credit",
                    new
                    {
                        intID = item.ID,
                        strCode = item.Code,
                        intSale = item.Sale,
                        bitStatus = item.Status,
                        strExpireDateFrom = item.ExpireDateFrom,
                        strExpireDateTo = item.ExpireDateTo,
                        intNail_ID = item.Nail_ID,
                        intMinAmountSaleOff = item.MinAmountSaleOff
                    }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(0).FirstOrDefault();
            return intID;
        }
    }
}