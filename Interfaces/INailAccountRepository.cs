﻿using System.Data.SqlClient;
using WebNails.Admin.Models;

namespace WebNails.Admin.Interfaces
{
    public interface INailAccountRepository : IConnection
    {
        NailAccount GetNailAccount(int ID);
        NailAccount GetNailAccount(string Username, string Password);
        NailAccount GetNailAccount(string Email);
        NailAccount GetNailAccountByUsername(string Username);
        int UpdateNailAccount(NailAccount item);
        int UpdatePassword(NailAccount item);
    }
}
