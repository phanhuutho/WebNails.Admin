using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNails.Admin.Models
{
    public class UserSite
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public int Nail_ID { get; set; }
        public bool Status { get; set; }
        public bool IsDelete { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public DateTime DateTimeDelete { get; set; }
    }
}