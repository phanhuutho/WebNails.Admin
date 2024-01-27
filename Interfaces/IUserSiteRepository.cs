using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebNails.Admin.Models;

namespace WebNails.Admin.Interfaces
{
    public interface IUserSiteRepository : IConnection
    {
        int DeleteUserSiteByID(int ID);
        UserSite GetUserSiteByID(int ID);
        UserSite GetUserSite(string Username, string Password, int NailID);
        UserSite GetUserSiteByUsername(string Username, int NailID);
        IEnumerable<UserSite> GetUserSites(DynamicParameters param);
        int SaveChange(UserSite item);
    }
}
