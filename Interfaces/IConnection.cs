using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebNails.Admin.Interfaces
{
    public interface IConnection
    {
        void InitConnection(SqlConnection sqlConnection);
    }
}
