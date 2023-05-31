using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebNails.Admin.Models;

namespace WebNails.Admin.Interfaces
{
    public interface INailSocialRepository : IConnection
    {
        NailSocial GetNailSocialByID(int ID);
        IEnumerable<NailSocial> GetNailSocial(DynamicParameters param);
        IEnumerable<NailSocial> GetNailSocialByNailID(int NailID);
        int DeleteNailSocial(int ID);
        int SaveChange(NailSocial item);
    }
}
