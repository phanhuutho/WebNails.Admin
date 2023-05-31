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
    public class NailCouponRepository : INailCouponRepository
    {
        protected SqlConnection _sqlConnection { get; set; }
        public int DeleteNailCoupon(int ID)
        {
            var intCount = _sqlConnection.Execute(@"spNailCoupon_DeleteByID", new { intID = ID }, commandType: CommandType.StoredProcedure);
            return intCount;
        }

        public NailCoupon GetNailCouponByID(int ID)
        {
            var objNailCoupon = this._sqlConnection.Query<NailCoupon>(@"spNailCoupon_GetNailCouponsByID", new { intID = ID }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(new NailCoupon()).FirstOrDefault();
            return objNailCoupon;
        }

        public IEnumerable<NailCoupon> GetNailCoupons(DynamicParameters param)
        {
            var objNailCoupons = this._sqlConnection.Query<NailCoupon>(@"spNailCoupon_GetNailCoupons", param, commandType: CommandType.StoredProcedure);
            return objNailCoupons;
        }

        public IEnumerable<NailCoupon> GetNailCouponsByNailID(int NailID)
        {
            var objNailCoupons = this._sqlConnection.Query<NailCoupon>(@"spNailCoupon_GetNailCouponsByNailID", new { intNail_ID = NailID }, commandType: CommandType.StoredProcedure);
            return objNailCoupons;
        }

        public void InitConnection(SqlConnection sqlConnection)
        {
            this._sqlConnection = sqlConnection;
        }

        public int SaveChange(NailCoupon item)
        {
            var intCount = _sqlConnection.Execute(@"spNailCoupon_Credit",
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