using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebNails.Admin.Models;

namespace WebNails.Admin.Interfaces
{
    public interface INailCodeSaleRepository : IConnection
    {
        int DeleteNailCodeSale(int ID);
        NailCodeSale GetNailCodeSaleByID(int ID);
        IEnumerable<NailCodeSale> GetNailCodeSales(DynamicParameters param);
        int SaveChange(NailCodeSale item);
    }
}
