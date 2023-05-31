using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebNails.Admin.Models;

namespace WebNails.Admin.Interfaces
{
    public interface IActionDetailRepository : IConnection
    {
        int ActionDetailLog(ActionDetail log);
    }
}
