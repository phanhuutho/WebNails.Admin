using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebNails.Admin.Models;

namespace WebNails.Admin.Interfaces
{
    public interface INailCouponRepository : IConnection
    {
        NailCoupon GetNailCouponByID(int ID);
        IEnumerable<NailCoupon> GetNailCoupons(DynamicParameters param);
        IEnumerable<NailCoupon> GetNailCouponsByNailID(int NailID);
        int DeleteNailCoupon(int ID);
        int SaveChange(NailCoupon item);
    }
}
