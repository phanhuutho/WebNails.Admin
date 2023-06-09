﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebNails.Admin.Models;

namespace WebNails.Admin.Interfaces
{
    public interface INailPricesRepository : IConnection
    {
        NailPrices GetNailPricesByID(int ID);
        IEnumerable<NailPrices> GetNailPrices(DynamicParameters param);
        IEnumerable<NailPrices> GetNailPricesByNailID(int NailID);
        int DeleteNailPrices(int ID);
        int SaveChange(NailPrices item);
    }
}
