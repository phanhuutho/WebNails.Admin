using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNails.Admin.Models
{
    public class ActionDetail
    {
        public string Table { get; set; } 
        public int UserID { get; set; }
        public string DataJson { get; set; }
        public string Description { get; set; }
    }
}