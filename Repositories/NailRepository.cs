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

        public Nail GetNailByDomain(string Domain)
        {
            var objNail = this._sqlConnection.Query<Nail>(@"spNail_GetNailByDomain", new { strDomain = Domain }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(new Nail()).FirstOrDefault();
            return objNail;
        }

        public IEnumerable<Nail> GetNails(DynamicParameters param)
        {
            var objNails = this._sqlConnection.Query<Nail>(@"spNail_GetNails", param, commandType: CommandType.StoredProcedure);
            return objNails;
        }

        public List<Nail> GetNails()
        {
            var objNails = this._sqlConnection.Query<Nail>(@"spNail_GetNailsActive", null, commandType: CommandType.StoredProcedure).ToList();
            return objNails;
        }

        public void InitConnection(SqlConnection sqlConnection)
        {
            this._sqlConnection = sqlConnection;
        }

        public int SaveChange(Nail item)
        {
            var intID = _sqlConnection.Query<int>(@"spNail_Credit",
                    new
                    {
                        intID = item.ID,
                        strDomain = item.Domain,
                        strLogo = item.Logo,
                        strName = item.Name,
                        strAddress = item.Address,
                        strHyperLinkTell = item.HyperLinkTell,
                        strTextTell = item.TextTell,
                        strGooglePlus = item.GooglePlus,
                        strLinkGoogleMapAddress = item.LinkGoogleMapAddress,
                        strLinkIFrameGoogleMap = item.LinkIFrameGoogleMap,
                        strLinkBookingAppointment = item.LinkBookingAppointment,
                        bitCoupons = item.Coupons,
                        strAboutUs = item.AboutUs,
                        strAboutUsHome = item.AboutUsHome,
                        strBusinessHours = item.BusinessHours,
                        intNailApiID = item.NailApi_ID
                    }, commandType: CommandType.StoredProcedure).DefaultIfEmpty(0).FirstOrDefault();
            return intID;
        }
    }
}