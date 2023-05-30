using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebNails.Admin.Models;

namespace WebNails.Admin.Interfaces
{
    public interface INailRepository : IConnection
    {
        Nail GetNailByID(int ID);
        IEnumerable<Nail> GetNails(DynamicParameters param);
        int SaveChange(Nail item);
    }
}
