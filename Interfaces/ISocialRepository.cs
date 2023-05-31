using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebNails.Admin.Models;

namespace WebNails.Admin.Interfaces
{
    public interface ISocialRepository : IConnection
    {
        IEnumerable<Social> GetSocials();
        IEnumerable<Social> GetSocials(int NailID);
    }
}
